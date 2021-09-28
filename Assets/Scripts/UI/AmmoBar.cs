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

    private void Update()
    {
        UpdateFillAmount();
    }

    private void UpdateFillAmount()
    {
        float currentOffSet = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffSet / maximumOffset;
        
        foreach (Image bar in ammoBars)
        {
            bar.fillAmount = fillAmount;    
        }
    }

    public void SetAmmo(int ammount)
    {
        current = ammount;
    }
    
    public void SetMaxAmmo(int maxAmmount, int currentAmmo)
    {
        maximum = maxAmmount;
        current = currentAmmo;
    }
}
