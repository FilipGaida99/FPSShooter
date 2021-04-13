using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTrace : MonoBehaviour
{
    public float fadingTime;
    public int steps;
    private LineRenderer renderer;
    // Start is called before the first frame update
    void Awake()
    {
        renderer = GetComponent<LineRenderer>();
    }

    public void DrawLine(Vector3 p1, Vector3 p2)
    {
        renderer.SetPosition(0, p1);
        renderer.SetPosition(1, p2);
        StartCoroutine(Fading());
    }

    public IEnumerator Fading()
    {
        for(int i =0; i<steps; i++)
        {
            yield return new WaitForSeconds(fadingTime/steps);
            renderer.startColor = new Color(renderer.startColor.r, renderer.startColor.g, renderer.startColor.b, renderer.startColor.a / 2);
            renderer.endColor = new Color(renderer.endColor.r, renderer.endColor.g, renderer.endColor.b, renderer.endColor.a / 2);
        }
        
        Destroy(gameObject);
        yield break;
    }
}
