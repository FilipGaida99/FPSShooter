using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletWeapon : MonoBehaviour, Weapon
{
    protected float reloadTime;

    [SerializeField]
    private float maxDistance = 100;
    [SerializeField]
    private readonly int maxMagazine = 10;
    [SerializeField]
    private readonly int maxBullets = 300;
    private int magazine;
    private int bullets;
    private bool reloading;
    private float leftToReload;
    [SerializeField]
    private float damage = 2;
    [SerializeField]
    private GameObject linePrefab;

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
    }

    virtual public void Reload()
    {
        if (!reloading)
        {
            reloading = true;
            leftToReload = ReloadTime;
        }
    }

    virtual public bool Shoot(Vector3 from, Vector3 direction)
    {
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

        RaycastHit hit;
        Vector3 hitPoint;
        if (Physics.Raycast(from, direction, out hit, maxDistance))
        {
            hitPoint = hit.point;
            //TODO cast Enemy.
        }
        else
        {
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

}
