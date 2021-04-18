using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : BulletWeapon
{
    [Header("Attack Parameters")]
    public float shootCount;
    public float shootTime;
    public float maxSprayAngle;

    [Header("Animator Mapping")]
    public string shootParameter = "Shoot";
    public string reloadParameter = "Reload";
    public string idleParameter = "Idle";

    protected int shootID;
    protected int reloadID;
    protected int idleID;

    private Animator animator;
    private float shootElapsedTime;

    public override float ReloadTime => 1f;

    public override bool WeaponReadyForShoot => base.WeaponReadyForShoot && shootElapsedTime > shootTime;

    public override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>(true);
        shootID = Animator.StringToHash(shootParameter);
        reloadID = Animator.StringToHash(reloadParameter);
        idleID = Animator.StringToHash(idleParameter);
        shootElapsedTime = shootTime;
    }

    public override void Update()
    {
        base.Update();
        shootElapsedTime += Time.deltaTime;
    }

    public override bool Reload()
    {
        if (base.Reload())
        {
            animator.SetTrigger(reloadID);
            shootElapsedTime = 0;
            return true;
        }
        return false;
    }

    public override bool Shoot(Vector3 from, Vector3 direction)
    {
        //Checking if can shoot.
        if (!WeaponReadyForReload || !CanShootOnSemiAuto || !WeaponReadyForShoot)
        {
            return false;
        }

        if (TryReloadIfEmpty())
        {
            return false;
        }

        Magazine--;
        wasReleased = false;

        for(int i = 0; i < shootCount; i++)
        {
            var randomizedDirection = RandomizeDirection(direction);
            Vector3 hitPoint = SingleShoot(from, randomizedDirection);
            DrawTrace(hitPoint);
        }

        animator.SetTrigger(shootID);
        shootElapsedTime = 0;
        RecoilMagnitude = 1;

        return true;
    }


    public override void OnShow()
    {
        base.OnShow();
        animator.SetTrigger(idleID);
    }

    protected Vector3 RandomizeDirection(Vector3 direction)
    {
        var randomX = Random.Range(-maxSprayAngle, maxSprayAngle);
        var randomY = Random.Range(-maxSprayAngle, maxSprayAngle);
        return Quaternion.Euler(randomX, randomY, 0) * direction;
    }

}
