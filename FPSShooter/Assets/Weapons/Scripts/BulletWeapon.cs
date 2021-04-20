using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public abstract class BulletWeapon : MonoBehaviour, Weapon, RecoilingWeapon
{
    [Header("Attack parameters")]
    public AnimationCurve recoilCurve;
    public AnimationCurve aimDispersionCurveX;
    public AnimationCurve aimDispersionCurveY;
    public AnimationCurve scaleAimImageToRecoil;
    public bool isSemiAuto = false;
    protected bool wasReleased = true;
    [SerializeField]
    private float recoilReturnSpeed = 1;
    private float recoilMagnitude = 0;
    private float lastRecoilMagnitude = 0;
    private Transform shootingPoint;

    [SerializeField]
    private float maxDistance = 100;
    [SerializeField]
    private uint maxMagazine = 10;
    [SerializeField]
    private uint maxBullets = 300;
    private uint magazine;
    private uint bullets;
    private bool reloading;
    private float leftToReload;
    [SerializeField]
    private float damage = 2;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject linePrefab;
    [SerializeField]
    private Sprite reload;
    [SerializeField]
    private Sprite aim;
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private AudioClip shootSound;
    [SerializeField]
    private AudioClip reloadSound;

    private AudioSource audioSource;

    private int layerMaskValue;

    public abstract float ReloadTime { get; }

    virtual public uint Magazine { get => magazine; set => magazine = value; }

    virtual public uint BulletsLeft { get => bullets; set => bullets = value; }

    virtual public float Damage { get => damage; set => damage = value; }

    virtual public bool IsReady => !reloading;

    virtual public Sprite Icon => icon;

    virtual public float RecoilMagnitude { get => recoilMagnitude; set => recoilMagnitude = Mathf.Clamp01(value); }

    virtual public bool WeaponReadyForReload { get => !reloading && bullets > 0; }

    virtual public bool CanShootOnSemiAuto { get => isSemiAuto ? wasReleased : true; }

    virtual public bool WeaponReadyForShoot { get => true; }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position with max shoot distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    virtual public void Awake()
    {
        if (linePrefab == null)
        {
            Debug.LogError("No line renderer in weapon");
        }
        magazine = maxMagazine;
        bullets = maxBullets;
        reloading = false;
        audioSource = GetComponent<AudioSource>();
        shootingPoint = GetComponentInChildren<ShootingPosition>().transform;
        LayerMask mask = LayerMask.GetMask("Character");
        layerMaskValue = mask.value;
        layerMaskValue = ~layerMaskValue;
    }

    virtual public void Update()
    {
        if (reloading)
        {
            UpdateReload();
        }
        else
        {
            leftToReload = ReloadTime;
        }

        if (isSemiAuto)
        {
            UpdateSemiAutoMechanism();
        }


        AdjustRecoil();
    }

    virtual public bool Reload()
    {
        if (WeaponReadyForReload)
        {
            audioSource.clip = reloadSound;
            audioSource.Play();

            InGameUIController.Instance.aimUI.SetImage(reload);
            reloading = true;
            leftToReload = ReloadTime;

            recoilMagnitude = 0;
            return true;
        }
        return false;
    }

    virtual public bool Shoot(Vector3 from, Vector3 direction)
    {
        //Checking if can shoot.
        if (!WeaponReadyForReload || !CanShootOnSemiAuto || !WeaponReadyForShoot)
        {
            return false; 
        }

        if (TryReloadIfEmpty())
        {
            return false;
        }

        magazine--;
        wasReleased = false;

        Vector3 hitPoint = SingleShoot(from, direction);
        DrawTrace(hitPoint);

        recoilMagnitude = 1;

        return true;
    }

    public bool ResupplyBullets()
    {
        if(bullets == maxBullets)
        {
            return false;
        }
        bullets = maxBullets;
        magazine = maxMagazine;
        return true;
    }

    virtual public void OnShow()
    {
        recoilMagnitude = 0;
        audioSource.Stop();
        InGameUIController.Instance.aimUI.SetImage(aim);
    }

    virtual public void OnHide()
    {
        //Stop reloading.
        reloading = false;
    }

    virtual protected Vector3 SingleShoot(Vector3 from, Vector3 direction)
    {
        audioSource.clip = shootSound;
        audioSource.Play();

        RaycastHit hit;
        Vector3 hitPoint;
        direction += GetDispersionVector();

        if (Physics.Raycast(from, direction, out hit, maxDistance, layerMaskValue))
        {
            hitPoint = hit.point;
            //TODO cast Enemy.
            var target = hit.collider.gameObject.GetComponent<DestroyAble>();
            if (target != null)
            {
                target.TakeDamage(damage, hitPoint);
            }
        }
        else
        {
            //There was no hit, just draw trace.
            hitPoint = from + direction * maxDistance;
        }

        return hitPoint;
    }

    virtual protected void DrawTrace(Vector3 hitPoint)
    {
        var traceObject = Instantiate(linePrefab);
        var trace = traceObject.GetComponent<ShootTrace>();
        trace.DrawLine(shootingPoint.position, hitPoint);
    }

    protected bool TryReloadIfEmpty()
    {
        if (magazine <= 0 && bullets > 0)
        {
            Reload();
            return true;
        }
        return false;
    }

    private void AdjustRecoil()
    {
        transform.Rotate(Vector3.back, recoilCurve.Evaluate(lastRecoilMagnitude), Space.Self);
        transform.Rotate(Vector3.forward, recoilCurve.Evaluate(recoilMagnitude), Space.Self);
        lastRecoilMagnitude = recoilMagnitude;
        recoilMagnitude -= recoilReturnSpeed * Time.deltaTime;
        recoilMagnitude = Mathf.Clamp01(recoilMagnitude);
        InGameUIController.Instance.aimUI.SetImageScale(scaleAimImageToRecoil.Evaluate(recoilMagnitude));
    }

    private Vector3 GetDispersionVector()
    {
        var result = new Vector3();
        var xValue = aimDispersionCurveX.Evaluate(recoilMagnitude);
        result.x = Random.Range(-xValue, xValue);
        result.y = Random.Range(0, aimDispersionCurveY.Evaluate(recoilMagnitude));
        result.z = 0;
        return result;
    }

    private void UpdateReload()
    {
        leftToReload -= Time.deltaTime;
        if (leftToReload < 0)
        {
            reloading = false;
            InGameUIController.Instance.aimUI.SetImage(aim);
            if (bullets - maxMagazine < 0)
            {
                magazine = bullets;
                bullets = 0;
            }
            else
            {
                bullets -= maxMagazine - magazine;
                magazine = maxMagazine;
            }
            InGameUIController.Instance.SetMagazineAndBullets(Magazine, bullets);
        }
    }

    private void UpdateSemiAutoMechanism()
    {
        if (!wasReleased && !Input.GetButton("Fire1"))
        {
            wasReleased = true;
        }
    }
}
