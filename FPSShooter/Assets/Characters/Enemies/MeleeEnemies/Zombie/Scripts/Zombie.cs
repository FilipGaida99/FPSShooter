using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MeleeEnemy
{
    [Header("Animator Mapping")]
    public string FallingDirectionAnimatorParameter = "IDOfFallingDirection";
    private int FallingDirectionID;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, AttackDistance);
    }

    public override void Awake()
    {
        base.Awake();

        var fallingDirection = Mathf.FloorToInt(Random.Range(0, 2));
        FallingDirectionID = Animator.StringToHash(FallingDirectionAnimatorParameter);
        animator.SetInteger(FallingDirectionID, fallingDirection);

        StartHuntPlayer(0.5f, 1f);
    }

    protected override void EndOfAttackCallback()
    {
        StartHuntPlayer(0f, 1f);
    }
}
