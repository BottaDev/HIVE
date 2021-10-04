using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerBullet : BasicBullet
{
    protected override void MakeDamage(Collider other)
    {
        // Enemy
        if (other.gameObject.layer == 7)
        {
            AI enemy = other.gameObject.GetComponentInParent<AI>();
            enemy.TakeDamage(damage);    
        }
        
        base.MakeDamage(other);
    }
}
