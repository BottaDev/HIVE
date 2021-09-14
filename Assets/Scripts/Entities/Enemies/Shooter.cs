using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : AI
{
    [Header("Shooter Parameters")]
    public float attackDistance = 20f;
    public float evadeDistance = 12f;
    public float evadeTime = 1.5f;

    private float _currentEvadeTime;

    protected override void Awake()
    {
        base.Awake();

        _currentEvadeTime = evadeTime;
    }

    protected override void Update()
    {
        base.Update();

        if (_playerDetected)
            MoveEnemy();
    }

    private void MoveEnemy()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > attackDistance)
        {
            ChasePlayer();    
        }
        else if (distance <= attackDistance && distance > evadeDistance)
        {
            if (ApplyFOV(_player.transform.position))
            {
                // Player is in range, and is being seen
                if (_currentAttackRate <= 0)
                    Attack();
                else
                    _currentAttackRate -= Time.deltaTime;
            }
            else
            {
                ChasePlayer();
            }
        }
        else if (distance <= evadeDistance)
        {
            if ( _currentEvadeTime > 0)
                EvadePlayer();
            else
                Attack();
        }
    }

    private void EvadePlayer()
    {
        _currentEvadeTime -= Time.deltaTime;
        
        Vector3 direction = transform.position - _player.transform.position;
        Vector3 newPos = transform.position + direction;
     
        _agent.isStopped = false;
        MoveToPosition(newPos);
    }
    
    private void ChasePlayer()
    {
        _currentEvadeTime = evadeTime;
        _agent.isStopped = false;
        MoveToPosition(_player.transform.position);
    }

    protected override void Attack()
    {
        _currentEvadeTime = evadeTime;
        _agent.isStopped = true;
        RotateTowards(_player.transform.position);
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, evadeDistance);
    }
}
