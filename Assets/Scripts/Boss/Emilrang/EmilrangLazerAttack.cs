using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmilrangLazerAttack : MonoBehaviour
{
    public int damage;
    public int invulnerabilityPeriod;
    public EmilrangBodyPart part;
    private bool readyToHitAgain = true;
    public bool activated;
    
    private void OnTriggerEnter(Collider other)
    {
        if (activated)
        {
            if (readyToHitAgain)
            {
                IDamageable obj = other.GetComponentInParent<IDamageable>() ?? other.GetComponentInChildren<IDamageable>();
    
                if (obj != null)
                {
                    Popup.Create(other.ClosestPoint(transform.position), damage.ToString(),Color.red);

                    obj.TakeDamage(damage);
                
                    readyToHitAgain = false;
                    Invoke(nameof(RefreshHit),invulnerabilityPeriod);
                
                    AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.LazerHit));
                }
            }
        }
    }

    void RefreshHit()
    {
        readyToHitAgain = true;
    }
}
