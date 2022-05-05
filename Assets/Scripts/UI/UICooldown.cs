using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICooldown : MonoBehaviour
{
    [SerializeField] private string eventType;
    [SerializeField] private Utilities_RadialTimerBar timer;
    [SerializeField] private bool startFull;
    
    private void Awake()
    {
        EventManager.Instance.Subscribe(eventType, Trigger);
    }
    
    private void Start()
    {
        if (startFull)
        {
            timer.Reset();
        }
    }

    private void Trigger(params object[] p)
    {
        float time = (float) p[0];
        timer.StartTimer(time);
    }
}
