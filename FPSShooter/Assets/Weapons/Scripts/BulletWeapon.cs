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
    [SerializeField]
    private float recoilReturnSpeed = 1;
    private float recoilMagnitude = 0;
    private float lastRecoilMagnitude = 0;
    private Transform shootingPoint;

    [SerializeField]
    private float maxDistance = 100;
    [SerializeField]
    private int maxMagazine = 10;
    [SerializeField]
    private int maxBullets = 300;
    private int magazine;
    private int bullets;
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
    private AudioClip shootSound;
    [SerializeField]
    private AudioClip reloadSound;

    private AudioSource audioSource;

    public abstract float ReloadTime { get; }

    virtual public int Magazine { get => magazine; set => magazine = value; }

    virtual public int BulletsLeft { get => bullets; set => bullets = value; }

    virtual public float Damage { get => damage; set => damage = value; }

    virtual public bool IsReady => !reloading;

    virtual public float RecoilMagnitude { get => recoilMagnitude; set => recoilMagnitude = Mathf.Clamp01(value); }

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
        shootingPoint = transform.Find("ShootingPoint");
    }

    virtual public void Reload()
    {
        if (!reloading && bullets > 0)
        {
            audioSource.clip = reloadSound;
            audioSource.Play();

            SetAimImage(reload);
            reloading = true;
            leftToReload = ReloadTime;

            recoilMagnitude = 0;
        }
    }

    virtual public bool Shoot(Vector3 from, Vector3 direction)
    {
        //Checking if can shoot.
        if (reloading || bullets <= 0)
        {
            return false; 
        }
        if(magazine <= 0 && bullets > 0)
        {
            Reload();
            return false;
        }
        magazine--;

        //Actual shoot
        audioSource.clip = shootSound;
        audioSource.Play();

        RaycastHit hit;
        Vector3 hitPoint;
        LayerMask mask = LayerMask.GetMask("Character");
        int maskValue = mask.value;
        maskValue = ~maskValue;
        direction += GetDispersionVector();
        if (Physics.Raycast(from, direction, out hit, maxDistance, maskValue))
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

        //Draw trace
        var traceObject = Instantiate(linePrefab);
        var trace = traceObject.GetComponent<ShootTrace>();
        var farest = from + direction * maxDistance;
        trace.DrawLine(shootingPoint.position, hitPoint);

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

    virtual public void Update()
    {
        if (reloading)
        {
            leftToReload -= Time.deltaTime;
            if(leftToReload < 0)
            {
                reloading = false;
                SetAimImage(aim);
                if(bullets - maxMagazine < 0)
                {
                    magazine = bullets;
                    bullets = 0;
                }
                else
                {
                    magazine = maxMagazine;
                    bullets -= maxMagazine;
                }
            }
        }
        AdjustRecoil();
    }

    //Todo: Do gamemanagera
    private void SetAimImage(Sprite sprite) {
        var aimingUI = GameObject.FindGameObjectWithTag("Aim").GetComponent<Image>();
        if (aimingUI != null)
        {
            aimingUI.sprite = sprite;
        }
    }

    //Todo: To też do gamemanagera
    private void SetAimImageScale(float scale)
    {
        var aimingUI = GameObject.FindGameObjectWithTag("Aim").GetComponent<Image>();
        if (aimingUI != null)
        {
            aimingUI.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    virtual public void OnShow()
    {
        recoilMagnitude = 0;
        SetAimImage(aim);
    }

    virtual public void OnHide()
    {
        //Stop reloading.
        reloading = false;
    }

    private void AdjustRecoil()
    {
        transform.Rotate(Vector3.back, recoilCurve.Evaluate(lastRecoilMagnitude), Space.Self);
        transform.Rotate(Vector3.forward, recoilCurve.Evaluate(recoilMagnitude), Space.Self);
        lastRecoilMagnitude = recoilMagnitude;
        recoilMagnitude -= recoilReturnSpeed * Time.deltaTime;
        recoilMagnitude = Mathf.Clamp01(recoilMagnitude);
        SetAimImageScale(scaleAimImageToRecoil.Evaluate(recoilMagnitude));
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
}
