using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{
    public int minimum = 0;
    public int maximum;
    public int current;
    
    public List<Image> ammoBars;

    public void UpdateFillAmount(int currentAmmo)
    {
        float currentOffSet = currentAmmo - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffSet / maximumOffset;
        
        foreach (Image bar in ammoBars)
        {
            bar.fillAmount = fillAmount;    
        }
    }
    
    public void SetMaxAmmo(int maxAmmount)
    {
        maximum = maxAmmount;
    }
}
