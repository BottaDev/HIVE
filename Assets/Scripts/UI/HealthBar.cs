using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fill;
    
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();

        EventManager.Instance?.Subscribe(EventManager.Events.OnLifeUpdated, OnLifeUpdated);
    }

    public void SetMaxHealt(float health)
    {
        _slider.maxValue = health;
        _slider.value = health;
    }

    public void SetHealth(float haelth)
    {
        _slider.value = haelth;
    }

    private void OnLifeUpdated(params object[] parameters)
    {
        SetHealth((float)parameters[0]);
    }
}
