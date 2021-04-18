using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StandardEnemy : Enemy
{
    [Header("Animator Mapping")]
    public string IsAliveAnimatorParameter = "IsAlive";
    public string IsAttackingAnimatorParameter = "IsAttacking";
    public string IsMovingAnimatorParameter = "IsMoving";
    public string IsRunningAnimatorParameter = "IsRunning";

    [Header("Moving Parameters")]
    public float walkingSpeed = 2.5f;
    public float runningSpeed = 6.5f;

    protected bool isMoving = false;
    protected bool isRunning = false;
    protected bool isAttacking = false;

    protected Animator animator;

    protected int IsAliveID;
    protected int IsAttackingID;
    protected int IsMovingID;
    protected int IsRunningID;

    private Coroutine activeHunt;

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        IsAliveID = Animator.StringToHash(IsAliveAnimatorParameter);
        IsAttackingID = Animator.StringToHash(IsAttackingAnimatorParameter);
        IsMovingID = Animator.StringToHash(IsMovingAnimatorParameter);
        IsRunningID = Animator.StringToHash(IsRunningAnimatorParameter);

        animator.SetBool(IsAliveID, IsAlive);
        animator.SetBool(IsMovingID, false);
        animator.SetBool(IsRunningID, false);
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

        UpdateMove();

        UpdateSound();
    }

    protected abstract void EndOfAttackCallback();

    public override void TakeDamage(float damage, Vector3 position)
    {
        base.TakeDamage(damage, position);
        animator.SetBool(IsAliveID, IsAlive);
    }

    virtual public void StartHuntPlayer(float walkStartTime, float runStartTime)
    {
        if(activeHunt != null)
        {
            StopCoroutine(activeHunt);
        }

        activeHunt = StartCoroutine(HuntPlayer(walkStartTime, runStartTime));
    }

    protected override void UpdateSound()
    {
        if (characterAudio.state == CharacterState.Idle && IsAlive)
        {
            if (isMoving)
            {
                characterAudio.state = CharacterState.Walking;
            }

            if (isRunning)
            {
                characterAudio.state = CharacterState.Running;
            }

            if (isAttacking)
            {
                characterAudio.state = CharacterState.Attacking;
            }
        }
        base.UpdateSound();
    }

    virtual protected void UpdateMove()
    {
        var playerPosition = player.transform.position;
        //Look at player
        transform.LookAt(new Vector3(playerPosition.x, transform.position.y, playerPosition.z));

        if (!isAttacking)
        {
            var directionXZ = new Vector2(playerPosition.x - transform.position.x, playerPosition.z - transform.position.z).normalized;
            var direction = new Vector3(directionXZ.x, -9.81f, directionXZ.y);
            var speed = 0.0f;
            if (isMoving)
            {
                speed = walkingSpeed;
            }
            if (isRunning)
            {
                speed = runningSpeed;
            }
            if (characterController.enabled)
                characterController.Move(speed * direction * Time.deltaTime);
        }
    }

    private IEnumerator HuntPlayer(float walkStartTime, float runStartTime)
    {
        isMoving = false;
        isRunning = false;
        yield return new WaitForSeconds(walkStartTime);
        isMoving = true;
        animator.SetBool(IsMovingID, true);
        yield return new WaitForSeconds(runStartTime);
        isRunning = true;
        animator.SetBool(IsRunningID, true);
    }

    virtual protected IEnumerator AttackRoutine()
    {
        Debug.LogWarning("Using default attack routine");
        yield return null;
    }
}
