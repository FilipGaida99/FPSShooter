using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeWeapon : MonoBehaviour, Weapon
{
    [SerializeField]
    private float damage = float.MaxValue;
    [SerializeField]
    private float maxSwingAngle = 90f;
    [SerializeField]
    private float swingTime = 0.5f;

    [SerializeField]
    private Sprite aim;

    private bool animating = false;
    private bool wasReleased = true;
    private bool combination = false;

    private Collider collider;

    virtual public int Magazine { get => 0; set {} }
    virtual public int BulletsLeft { get => 0; set { } }
    virtual public float Damage { get => damage; set => damage = value; }

    virtual public bool IsReady => !animating;

    public void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public void OnEnable()
    {
        animating = false;
        wasReleased = true;
        combination = false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        var hitbox = collision.gameObject.GetComponent<Hitbox>();
        if(hitbox != null)
        {
            hitbox.Hit(Damage, collider.ClosestPoint(hitbox.transform.position));
        }
    }

    virtual public void Reload()
    {
        
    }

    virtual public bool ResupplyBullets()
    {
        return false;
    }

    virtual public bool Shoot(Vector3 from, Vector3 direction)
    {

        if (!animating && wasReleased) {
            animating = true;
            wasReleased = false;
            combination = false;
            StartCoroutine(AnimateSwing());
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

    virtual public void OnShow()
    {
        SetAimImage(aim);
    }

    virtual public void OnHide()
    {

    }

    //Todo: Do gamemanagera
    private void SetAimImage(Sprite sprite)
    {
        var aimingUI = GameObject.FindGameObjectWithTag("Aim").GetComponent<Image>();
        if (aimingUI != null)
        {
            aimingUI.sprite = sprite;
        }
    }

    #region Animations
    virtual protected IEnumerator AnimateSwing()
    {
        //TODO:Turn on/off colisions.
        var startPosition = transform.localPosition;
        var startRotation = transform.localRotation;
        //Start
        {
            float angle = 90;
            float time = swingTime / 5;
            float actualAngle = 0;
            while (Mathf.Abs(actualAngle) < Mathf.Abs(angle))
            {
                float angleDelta = angle * Time.deltaTime / time;
                actualAngle += angleDelta;
                transform.Rotate(Vector3.up, angleDelta);

                yield return null;
            }
        }
        //Slash
        {

            float angle = -maxSwingAngle;
            float time = 4 * swingTime / 5;
            float actualAngle = 0;
            while (Mathf.Abs(actualAngle) < Mathf.Abs(angle))
            {
                float angleDelta = angle * Time.deltaTime / time;
                actualAngle += angleDelta;
                transform.RotateAround(transform.parent.position, transform.parent.up, angleDelta);

                yield return null;
            }

        }
        //If combination - return with hit.
        if (combination)
        {
            combination = false;
            {

                float angleX = 150;
                float angleY = -50;
                float time = swingTime / 5;
                float actualAngle = 0;
                while (Mathf.Abs(actualAngle) < Mathf.Abs(angleX))
                {
                    float angleDelta = angleX * Time.deltaTime / time;
                    actualAngle += angleDelta;

                    transform.Rotate(Vector3.right, angleDelta);
                    transform.Rotate(transform.parent.up, angleY * Time.deltaTime / time);
                    yield return null;
                }
            }

            {
                float angle = maxSwingAngle;
                float time = 4 * swingTime / 5;
                float actualAngle = 0;
                while (Mathf.Abs(actualAngle) < Mathf.Abs(angle))
                {
                    float angleDelta = angle * Time.deltaTime / time;
                    actualAngle += angleDelta;

                    transform.RotateAround(transform.parent.position, transform.parent.up, angleDelta);
                    yield return null;
                }
            }
        }

        //Return
        var endPosition = transform.localPosition;
        var endRotation = transform.localRotation;
        {
            int steps = 20;
            float time = 0.1f;
            float elapsedTime = 0;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(endPosition, startPosition, elapsedTime/time);
                transform.localRotation = Quaternion.Slerp(endRotation, startRotation, elapsedTime/time);
                yield return new WaitForSeconds(time / steps);
            }
            transform.localPosition = startPosition;
            transform.localRotation = startRotation;
        }

        animating = false;
        transform.localRotation = startRotation;
        transform.localPosition = startPosition;
        yield break;
    }

    #endregion
}
