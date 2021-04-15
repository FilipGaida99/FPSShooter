using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TintCameraPP : MonoBehaviour
{
    // Start is called before the first frame update

    public Material material;
    public Color color;
    [Range(0f,1f)]
    public float tintValue = 0;
    public bool isAutoDim;
    public float dimSpeed = 1;

    private int tintValueID;

    private void Awake()
    {
        tintValueID = Shader.PropertyToID("_intensity");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.color = color;
        material.SetFloat(tintValueID, tintValue);
        Graphics.Blit(source, destination, material);

    }

    // Update is called once per frame
    void Update()
    {
        if (isAutoDim)
        {
            if (tintValue < 0.001)
            {
                tintValue = 0;
                return;
            }
            tintValue -= tintValue * dimSpeed * Time.deltaTime;
        }
    }
}
