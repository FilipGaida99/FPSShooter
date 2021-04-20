using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image status;
    public Gradient barGradient;

    private Slider fillingSlider;

    public void Awake()
    {
        fillingSlider = GetComponent<Slider>();
    }

    public void SetHealth(float health, float maxHealth)
    {
        float healthRatio = Mathf.Clamp(health, 0, maxHealth) / maxHealth;
        fillingSlider.value = healthRatio;
        status.color = barGradient.Evaluate(healthRatio);
    }

}
