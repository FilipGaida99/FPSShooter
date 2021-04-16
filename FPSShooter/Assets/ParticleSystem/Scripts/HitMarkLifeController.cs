using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class HitMarkLifeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var part = GetComponent<ParticleSystem>();
        part.Play();
        Destroy(gameObject, part.main.duration);
    }
}
