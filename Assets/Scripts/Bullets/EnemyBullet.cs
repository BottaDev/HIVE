using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : BasicBullet
{
    protected override void MakeDamage(Collider other)
    {
        // Player
        if (other.gameObject.layer == 8)
            EventManager.Instance.Trigger("OnPlayerDamaged", damage);
        
        base.MakeDamage(other);
    }
}
