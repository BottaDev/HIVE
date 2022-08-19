using System;
using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using UnityEngine;
using UnityEngine.Events;

public class EmilrangLazerAttack : MonoBehaviour
{
    public int damage;
    public int invulnerabilityPeriod;
    private bool readyToHitAgain = true;
    public bool activated;
    
    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;
    private void OnTriggerEnter(Collider other)
    {
        if (activated)
        {
            if (readyToHitAgain)
            {
                IDamageable obj = other.GetComponentInParent<IDamageable>() ?? other.GetComponentInChildren<IDamageable>();
    
                if (obj != null)
                {
                    Popup.Create(other.ClosestPoint(transform.position), damage.ToString(),KamColor.purple);

                    obj.TakeDamage(damage);
                
                    readyToHitAgain = false;
                    Invoke(nameof(RefreshHit),invulnerabilityPeriod);
                
                    AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.LazerHit));
                }
            }
        }
    }

    public void SetActive(bool state)
    {
        activated = state;

        if (state)
        {
            OnActivated.Invoke();
        }
        else
        {
            OnDeactivated.Invoke();
        }
    }
    
    void RefreshHit()
    {
        readyToHitAgain = true;
    }
}
