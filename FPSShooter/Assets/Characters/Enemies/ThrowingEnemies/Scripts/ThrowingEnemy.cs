using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrowingEnemy : StandardEnemy
{
    public List<GameObject> projectilePrefabs;
    public float TimeBetweenAttacks = 1;

    private Transform shootingPosition;

    [SerializeField]
    private float throwOffsetTime = 0;
    [SerializeField]
    private float attackEndTimeAfterThrow = 0;

    private float elapsedTimeFromLastAttack = 0;

    public override void Awake()
    {
        base.Awake();
        shootingPosition = GetComponentInChildren<ShootingPosition>().transform;
        StartHuntPlayer(0.5f, 10f);
    }

    public override void Update()
    {
        base.Update();
        elapsedTimeFromLastAttack += Time.deltaTime;

    }

    public override void Attack()
    {
        if (CanAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    virtual public bool NoObstacles {
        get {
            LayerMask mask = LayerMask.GetMask("Character");
            int maskValue = mask.value;
            maskValue = ~maskValue;
            return !Physics.Linecast(transform.position, player.transform.position, maskValue);
        }
    }

    virtual protected Vector3 GetProjectileDirection()
    {
        return (player.transform.position - shootingPosition.position).normalized;
    }



    protected override IEnumerator AttackRoutine()
    {
        animator.SetTrigger(IsAttackingID);
        isAttacking = true;
        yield return new WaitForSeconds(throwOffsetTime);

        if (IsAlive)
        {
            elapsedTimeFromLastAttack = 0;
            var projectile = Projectile.ThrowProjectile(GetRandomProjectile(), shootingPosition.position, GetProjectileDirection());
        }

        yield return new WaitForSeconds(attackEndTimeAfterThrow);
        isAttacking = false;
    }

    virtual protected bool CanAttack => !isAttacking && NoObstacles && elapsedTimeFromLastAttack > TimeBetweenAttacks && IsPlayerInDistance(Vector3.up);

    private GameObject GetRandomProjectile()
    {
        return projectilePrefabs[Random.Range(0, projectilePrefabs.Count)];
    }
}
