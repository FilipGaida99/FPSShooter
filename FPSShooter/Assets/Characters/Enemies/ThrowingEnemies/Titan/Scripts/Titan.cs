using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titan : ThrowingEnemy
{
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, AttackDistance);
    }

    public override void Awake()
    {
        base.Awake();
        StartHuntPlayer(0.5f, 3f);
    }

    protected override void EndOfAttackCallback()
    {
        StartHuntPlayer(0, 1f);
    }
}
