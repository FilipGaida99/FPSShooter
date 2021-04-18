using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : BulletWeapon
{
    private Animation reloadAnimation;

    public override float ReloadTime { 
        get { 
            if(reloadAnimation == null)
            {
                return 4.0f;
            }
            return reloadAnimation.clip.length; 
        } 
    }

    public override void Awake()
    {
        base.Awake();
        reloadAnimation = GetComponent<Animation>();
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

    public override void OnShow()
    {
        base.OnShow(); 
        //Reset animation.
        reloadAnimation.Play();
        StartCoroutine(StopAnimation());
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    private IEnumerator StopAnimation()
    {
        yield return null;
        reloadAnimation.Stop();
        yield break;
    }
}
