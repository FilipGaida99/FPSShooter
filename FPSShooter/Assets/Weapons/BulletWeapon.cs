using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public abstract class BulletWeapon : MonoBehaviour, Weapon
{
    [Header("Attack parameters")]
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

    public int Magazine { get => magazine; set => magazine = value; }
    public int BulletsLeft { get => bullets; set => bullets = value; }
    public float Damage { get => damage; set => damage = value; }

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
        if (Physics.Raycast(from, direction, out hit, maxDistance))
        {
            hitPoint = hit.point;
            //TODO cast Enemy.
        }
        else
        {
            //There was no hit, just draw trace.
            hitPoint = from + direction * maxDistance;
        }
        
        var traceObject = Instantiate(linePrefab);
        var trace = traceObject.GetComponent<ShootTrace>();
        var farest = from + direction * maxDistance;
        trace.DrawLine(transform.Find("ShootingPoint").position, hitPoint);

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
    }

    private void SetAimImage(Sprite sprite) {
        var aimingUI = GameObject.FindGameObjectWithTag("Aim").GetComponent<Image>();
        if (aimingUI != null)
        {
            aimingUI.sprite = sprite;
        }
    }

    virtual public void OnChoose()
    {
        SetAimImage(aim);
    }

    virtual public void OnHide()
    {
        //Stop reloading.
        reloading = false;
    }
}
