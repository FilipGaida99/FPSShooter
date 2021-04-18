using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeEnemy : StandardEnemy
{
    [Header("Attack Parameters")]
    [SerializeField]
    private float damage = 3;
    [SerializeField]
    private float attackOffsetTime = 0.7f;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    override protected IEnumerator AttackRoutine()
    {
        animator.SetTrigger(IsAttackingID);
        isAttacking = true;

        yield return new WaitForSeconds(attackOffsetTime);
        if (IsPlayerInDistance(Vector3.up) && IsAlive)
        {
            //Deal damage
            player.TakeDamage(damage, player.transform.position);
        }
        isAttacking = false;
        yield break;
    }
}
