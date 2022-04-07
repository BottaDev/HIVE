using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerBullet : BasicBullet
{
    protected override void OnTriggerEnter(Collider other)
    {
        IDamagable obj = other.GetComponentInParent<IDamagable>();
        if(obj != null)
        {
            obj.TakeDamage(damage);
            Crosshair.instance.Hit();
        }
        
        base.OnTriggerEnter(other);
    }
}
