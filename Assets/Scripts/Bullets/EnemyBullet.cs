using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : BasicBullet
{
    protected override void Update()
    {
        base.Update();

        if (Physics.Raycast(transform.position, transform.forward, out hit, distance, mask))
        {
            if (hit.transform.gameObject.layer == 8)
            {
                Player enemy = hit.transform.gameObject.GetComponentInParent<Player>();
                enemy.TakeDamage(damage);
            }
            
            MakeDamage();
        }
    }
}
