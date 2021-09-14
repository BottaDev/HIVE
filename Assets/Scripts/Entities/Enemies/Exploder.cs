using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using UnityEngine;

public class Exploder : AI
{
    [Header("Enemy Parameters")]
    public float explodeDistance = 5f;
    
    protected override void Update()
    {
        base.Update();

        if (_playerDetected)
            ChasePlayer();
    }
    
    private void ChasePlayer()
    {
        MoveToPosition(_player.transform.position);

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= explodeDistance)
            Attack();
    }
    
    protected override void Attack()
    {
        RotateTowards(_player.transform.position);
        Debug.Log("EXPLODE!");
        Destroy(gameObject, 0.5f);   
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeDistance);
    }
}
