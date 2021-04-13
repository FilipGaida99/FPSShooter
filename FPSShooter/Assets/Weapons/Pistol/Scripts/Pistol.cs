using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : BulletWeapon
{
    private bool wasReleased = true;
    private new Animation animation;

    public override float ReloadTime { 
        get { 
            if(animation == null)
            {
                return 4.0f;
            }
            return animation.clip.length; 
        } 
    }

    override public void Awake()
    {
        base.Awake();
        animation = GetComponent<Animation>();
    }

    public override void Reload()
    {
        base.Reload();
        animation.Play();
    }

    override public bool Shoot(Vector3 from, Vector3 direction)
    {
        if (wasReleased)
        {
            wasReleased = false;
            return base.Shoot(from, direction);
        }
        return false;
    }



    override public void Update()
    {
        base.Update();
        if(!wasReleased && !Input.GetMouseButton(0))
        {
            wasReleased = true;
        }
    }

}
