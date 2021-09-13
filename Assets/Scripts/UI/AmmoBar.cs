using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{
    public Image leftBar;
    public Image rightBar;

    private float _cooldownRate;
    private bool _inCooldown;
    
    private void Update()
    {
        if (!_inCooldown)
        {
            leftBar.fillAmount += 1 / _cooldownRate * Time.deltaTime;
            rightBar.fillAmount += 1 / _cooldownRate * Time.deltaTime;
            
            if (leftBar.fillAmount >= 1)
                leftBar.fillAmount = 1;
            
            if (rightBar.fillAmount >= 1)
                rightBar.fillAmount = 1;
        }
    }
    
    public IEnumerator StartCoolDown(float waitTime, float cooldownRate)
    {
        leftBar.fillAmount = 0f;
        rightBar.fillAmount = 0f;

        _cooldownRate = cooldownRate;
        
        _inCooldown = true;
        
        yield return new WaitForSeconds(waitTime);
        
        _inCooldown = false;
    }
    
}
