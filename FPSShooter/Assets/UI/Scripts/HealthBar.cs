using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image status;
    public Gradient barGradient;

    public float changeSpeed = 0.1f;
    public float changeThreshold = 0.001f;

    private Slider fillingSlider;
    private float healthRatio;

    public void Awake()
    {
        fillingSlider = GetComponent<Slider>();
    }

    public void Update()
    {
        if(Mathf.Abs(fillingSlider.value - healthRatio) > changeThreshold)
        {
            fillingSlider.value += (fillingSlider.value < healthRatio ? changeSpeed : -changeSpeed) * Time.deltaTime;
        }
        else
        {
            fillingSlider.value = healthRatio;
        }
        status.color = barGradient.Evaluate(fillingSlider.value);
    }

    public void SetHealth(float health, float maxHealth)
    {
        healthRatio = Mathf.Clamp(health, 0, maxHealth) / maxHealth;
    }

}
