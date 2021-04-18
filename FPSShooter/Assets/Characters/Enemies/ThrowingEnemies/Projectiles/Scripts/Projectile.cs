using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, DestroyAble
{
    public GameObject hitmark;

    public float speed = 1;
    public float maxDistance = 10;
    public float damage;
    public float durability;

    protected Vector3 direction;

    protected Rigidbody rigidbody;
    private float traveledDistance = 0;
    

    public static GameObject ThrowProjectile(GameObject prefab, Vector3 position, Vector3 direction)
    {
        var newProjectile = Instantiate(prefab, position, Quaternion.identity).GetComponent<Projectile>();
        newProjectile.SetRigidbody(direction);
        return newProjectile.gameObject;
    }

    virtual public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    virtual public void Update()
    {
        traveledDistance += rigidbody.velocity.magnitude * Time.deltaTime;
        if(traveledDistance > maxDistance)
        {
            rigidbody.useGravity = true;
        }
    }

    virtual public void OnCollisionEnter(Collision collision)
    {
        //Don't destroy on collision with enemy and other projectiles.
        if (collision.gameObject.GetComponent<Enemy>() || collision.gameObject.GetComponent<Projectile>())
        {
            return;
        }

        var targetToDestroy = collision.gameObject.GetComponent<DestroyAble>();
        if(targetToDestroy != null)
        {
            targetToDestroy.TakeDamage(damage, collision.collider.ClosestPoint(transform.position));
        }
        Die();
    }

    virtual public void SetRigidbody(Vector3 targetDirection)
    {
        direction = targetDirection;
        rigidbody.velocity = direction * speed;
    }

    public void TakeDamage(float damage, Vector3 hitPoint)
    {
        durability -= damage;
        if(durability < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(hitmark != null)
        {
            Instantiate(hitmark, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
}
