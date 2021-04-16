using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    [SerializeField]
    private float attackMultiplier = 1;

    private Collider collider;

    // Start is called before the first frame update
    void Awake()
    {
        collider = GetComponent<Collider>();

        if(collider == null)
        {
            Debug.LogError("No collider in hitbox:" + gameObject.name);
        }
    }

    public void Hit(float damage, Vector3 hitPoint)
    {
        var enemy = GetComponentInParent<Enemy>();
        if(enemy != null)
        {
            enemy.TakeDamage(damage * attackMultiplier, hitPoint);
        }
        else
        {
            Debug.LogError("No enemy for hitbox");
        }
    }

    private void Reset()
    {
        attackMultiplier = 1;
    }


}
