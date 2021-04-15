using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour, Weapon
{
    [SerializeField]
    private float damage = float.MaxValue;
    [SerializeField]
    private float maxSwingAngle = 90f;
    [SerializeField]
    private float swingTime = 0.5f;



    private bool animating = false;
    private bool wasReleased = true;
    private bool combination = false;

    public int Magazine { get => 0; set {} }
    public int BulletsLeft { get => 0; set { } }
    public float Damage { get => damage; set => damage = value; }



    public void OnEnable()
    {
        animating = false;
        wasReleased = true;
        combination = false;
    }

    public void Reload()
    {
        
    }

    public bool ResupplyBullets()
    {
        return false;
    }

    public bool Shoot(Vector3 from, Vector3 direction)
    {

        if (!animating && wasReleased) {
            animating = true;
            wasReleased = false;
            combination = false;
            StartCoroutine(Animate());
        }
        else if(animating && wasReleased)
        {
            combination = true;
        }
        return true;
    }

    virtual public void Update()
    {
        if (!wasReleased && !Input.GetMouseButton(0))
        {
            wasReleased = true;
        }
    }

    private IEnumerator Animate()
    {
        //TODO:Turn on/off colisions.
        var startPosition = transform.localPosition;
        var startRotation = transform.localRotation;
        //Start
        {
            int steps = 10;
            float angle = 90;
            float time = swingTime / 5;
            for (int i = 0; i < steps; i++)
            {
                transform.Rotate(Vector3.up, angle / steps);
                yield return new WaitForSeconds(time / steps);
            }
        }
        //Slash
        {
            int steps = 100;
            float angle = -maxSwingAngle;
            float time = 4 * swingTime / 5;
            for (int i = 0; i < steps; i++)
            {
                transform.RotateAround(transform.parent.position, transform.parent.up, angle / steps);
                yield return new WaitForSeconds(time / steps);
            }

          
        }
        //If combination - return with hit.
        if (combination)
        {
            combination = false;
            {
                int steps = 10;
                float angle = 180;
                float time = swingTime / 5;
                for (int i = 0; i < steps; i++)
                {
                    transform.Rotate(Vector3.right, angle / steps);
                    transform.Rotate(transform.parent.up, -50 / steps);
                    yield return new WaitForSeconds(time / steps);
                }
            }

            {
                int steps = 100;
                float angle = maxSwingAngle;
                float time = 4 * swingTime / 5;
                for (int i = 0; i < steps; i++)
                {
                    transform.RotateAround(transform.parent.position, transform.parent.up, angle / steps);
                    yield return new WaitForSeconds(time / steps);
                }

            }
        }

        //Return
        var endPosition = transform.localPosition;
        var endRotation = transform.localRotation;
        {
            int steps = 20;
            float time = 0.2f;
            for (int i = 0; i < steps; i++)
            {
                transform.localPosition = Vector3.Lerp(endPosition, startPosition, i / (float)steps);
                transform.localRotation = Quaternion.Slerp(endRotation, startRotation, i / (float)steps);
                yield return new WaitForSeconds(time / steps);
            }
        }

        animating = false;
        transform.localRotation = startRotation;
        transform.localPosition = startPosition;
        yield break;
    }

    public void OnChoose()
    {
        
    }

    public void OnHide()
    {
        
    }
}
