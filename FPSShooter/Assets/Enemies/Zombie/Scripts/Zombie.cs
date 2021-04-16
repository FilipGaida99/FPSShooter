using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
   
    [Header("Animator Mapping")]
    public string IsAliveAnimatorParameter = "IsAlive";
    public string IsAttackingAnimatorParameter = "IsAttacking";
    public string IsMovingAnimatorParameter = "IsMoving";
    public string IsRunningAnimatorParameter = "IsRunning";
    public string FallingDirectionAnimatorParameter = "IDOfFallingDirection";

    [Header("Moving Parameters")]
    public float walkingSpeed = 2.5f;
    public float runningSpeed = 6.5f;

    private int IsAliveID;
    private int IsAttackingID;
    private int IsMovingID;
    private int IsRunningID;
    private int FallingDirectionID;

    [Header("Attack Parameters")]
    [SerializeField]
    private float damage = 3;

    private Animator animator;

    private bool isMoving = false;
    private bool isRunning = false;
    private bool isAttacking = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, AttackDistance);
    }

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        var fallingDirection = Mathf.FloorToInt(Random.Range(0.0f, 1.99f));

        IsAliveID = Animator.StringToHash(IsAliveAnimatorParameter);
        IsAttackingID = Animator.StringToHash(IsAttackingAnimatorParameter);
        IsMovingID = Animator.StringToHash(IsMovingAnimatorParameter);
        IsRunningID = Animator.StringToHash(IsRunningAnimatorParameter);
        FallingDirectionID = Animator.StringToHash(FallingDirectionAnimatorParameter);

        animator.SetInteger(FallingDirectionID, fallingDirection);
        animator.SetBool(IsAliveID, IsAlive);
        animator.SetBool(IsMovingID, false);
        animator.SetBool(IsRunningID, false);
        StartCoroutine(HuntPlayer(player));
    }

    public override void Update()
    {
        base.Update();

        if (player == null)
        {
            isMoving = false;
            isRunning = false;
            return;
        }

        var playerPosition = player.transform.position;

        

        //Look at player
        transform.LookAt(new Vector3(playerPosition.x, transform.position.y, playerPosition.z));
        if (!isAttacking) {
            var directionXZ = new Vector2(playerPosition.x - transform.position.x, playerPosition.z - transform.position.z).normalized;
            var direction = new Vector3(directionXZ.x, 0, directionXZ.y);
            var speed = 0.0f;
            if (isMoving)
            {
                speed = walkingSpeed;
            }
            if (isRunning)
            {
                speed = runningSpeed;
            }
            if(characterController.enabled)
                characterController.Move(speed * direction * Time.deltaTime);
        }
    }

    public override void TakeDamage(float damage, Vector3 position)
    {
        base.TakeDamage(damage, position);
        animator.SetBool(IsAliveID, IsAlive);
    }

    private IEnumerator HuntPlayer(Player player)
    {
        yield return new WaitForSeconds(0.5f);
        isMoving = true;
        animator.SetBool(IsMovingID, true);
        yield return new WaitForSeconds(2f);
        isRunning = true;
        animator.SetBool(IsRunningID, true);
    }

    public override void Attack()
    {
        if (!isAttacking)
        {

            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        animator.SetTrigger(IsAttackingID);
        isAttacking = true;

        float attackOffset = 0.7f;

        yield return new WaitForSeconds(attackOffset);
        if (IsPlayerInDistance(Vector3.up) && IsAlive)
        {
            //Deal damage
            player.DealDamage(damage);
        }
        isAttacking = false;
        yield break;
    }
}
