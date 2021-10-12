using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerBullet : BasicBullet
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Physics.Raycast(transform.position, transform.forward, out hit, distance, mask))
        {
            if (hit.transform.gameObject.layer == 7)
            {
                AI enemy = hit.transform.gameObject.GetComponentInParent<AI>();
                enemy.TakeDamage(damage);
            }
            
            MakeDamage();
        }
    }
}
