using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Gradient gradient;
    public Image fill;
    
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetMaxHealt(float health)
    {
        _slider.maxValue = health;
        _slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float haelth)
    {
        _slider.value = haelth;

        fill.color = gradient.Evaluate(_slider.normalizedValue);
    }
}
