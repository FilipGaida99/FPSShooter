using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SupplyCrate : MonoBehaviour
{
    public AudioClip pickupSound;

    public float fallingSpeed = 1;

    public bool IsGrounded => collisionCount > 0;

    private AudioSource audioSource;
    private Rigidbody rigidbody;
    private int collisionCount;

    public abstract void Resupply(Player player);

    public virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = new Vector3(0, -fallingSpeed, 0);
        collisionCount = 0;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = pickupSound;
    }


    protected void MarkAsUsed()
    {
        StartCoroutine(MarkingAsUsed());
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionCount++;
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionCount--;
    }

    private IEnumerator MarkingAsUsed()
    {
        audioSource.Play();
        yield return new WaitForSeconds(pickupSound.length);
        Destroy(gameObject);
    }

}
