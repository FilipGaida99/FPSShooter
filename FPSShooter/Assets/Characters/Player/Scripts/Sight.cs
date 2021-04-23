using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{

    public float zoomedFov = 20;
    public float normalFov =0;
    public float zoomSpeed = 10;

    private Camera camera;
    private float zoomScale;
    private bool isZoomed;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        if(normalFov == 0)
        {
            normalFov = camera.fieldOfView;
        }
    }

    public void Zoom()
    {
        isZoomed = true;
    }

    public void Return()
    {
        isZoomed = false;
    }

    private void Update()
    {
        zoomScale += (isZoomed ? zoomSpeed : -zoomSpeed) * Time.deltaTime;
        zoomScale = Mathf.Clamp01(zoomScale);
        camera.fieldOfView = Mathf.Lerp(normalFov, zoomedFov, zoomScale);

    }
}
