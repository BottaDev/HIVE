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

    private void Awake()
    {
        EventManager.Instance.Subscribe(EventManager.Events.OnPlayerUpdateAmmo, UpdateBar);
    }

    public void UpdateBar(params object[] p)
    {
        int currentAmmo = (int)p[0];
        int maxAmmount = (int)p[1];
        
        UpdateFillAmount(currentAmmo);
        SetMaxAmmo(maxAmmount);
    }
    
    void UpdateFillAmount(int currentAmmo)
    {
        float currentOffSet = currentAmmo - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffSet / maximumOffset;
        
        foreach (Image bar in ammoBars)
        {
            bar.fillAmount = fillAmount;    
        }
    }
    
    void SetMaxAmmo(int maxAmmount)
    {
        maximum = maxAmmount;
    }
}
