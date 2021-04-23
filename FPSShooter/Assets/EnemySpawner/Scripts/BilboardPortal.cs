using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilboardPortal : MonoBehaviour
{
    public float rotationSpeed = 2f;

    private float rotation = 0;
    void Update()
    {
        transform.rotation = Quaternion.identity;
        transform.LookAt(GameManager.Instance.mainCamera.transform.position);
        transform.Rotate(90, 0, 0);
        rotation += rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotation, Space.Self);
    }
}
