using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingProjectile : Projectile
{
    public override void SetRigidbody(Vector3 targetDirection)
    {
        base.SetRigidbody(targetDirection);
        var angularRotation = Random.Range(0f, 2f);
        rigidbody.angularVelocity = new Vector3(angularRotation, angularRotation, angularRotation);
    }
}
