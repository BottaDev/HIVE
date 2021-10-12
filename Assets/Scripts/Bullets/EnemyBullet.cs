using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : BasicBullet
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Physics.Raycast(transform.position, transform.forward, out hit, distance, mask))
        {
            if (hit.transform.gameObject.layer == 8)
            {
                Player enemy = hit.transform.gameObject.GetComponent<Player>();
                enemy.TakeDamage(damage);
            }
            
            Impact();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.TakeDamage(damage);
        }
        
        base.OnTriggerEnter(other);
    }
}
