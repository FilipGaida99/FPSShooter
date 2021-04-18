using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : BulletWeapon
{
    [Header("Automatic Shoot Parameters")]
    public float lockTime;

    private Animation reloadAnimation;

    private float lockElapsedTime;

    public override float ReloadTime => reloadAnimation.clip.length;

    public override bool WeaponReadyForShoot => base.WeaponReadyForShoot && lockElapsedTime > lockTime;

    public override void Awake()
    {
        base.Awake();
        reloadAnimation = GetComponent<Animation>();
    }

    public override void Update()
    {
        base.Update();
        lockElapsedTime += Time.deltaTime;
    }

    public override bool Reload()
    {
        if (base.Reload()) 
        {
            reloadAnimation.Play();
            return true;
        }
        return false;
    }

    public override bool Shoot(Vector3 from, Vector3 direction)
    {  
        if(base.Shoot(from, direction))
        {
            lockElapsedTime = 0;
        }
        return false;
    }

    public override void OnShow()
    {
        base.OnShow();
        //Reset animation.
        reloadAnimation.Play();
        StartCoroutine(StopAnimation());
    }

    private IEnumerator StopAnimation()
    {
        yield return null;
        reloadAnimation.Stop();
        yield break;
    }
}
