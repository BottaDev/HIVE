using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Utilities_RadialProgressBar : Utilities_ProgressBar
{
    public Image radialImage;

    protected override void UpdateBar()
    {
        float range = maxValue - minValue;
        float newFill = (currentValue - minValue) / range;
        radialImage.fillAmount = newFill;
    }
}

