using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

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

        EventManager.Instance.Subscribe("OnEnemyDamaged", OnEnemyDamaged);
        
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
        else
        {
            // In Attack distance
            if (distance <= evadeDistance)
            {
                _currentEvadeTime -= Time.deltaTime;
                
                if ( _currentEvadeTime > 0)
                    EvadePlayer();
                else
                    Attack();
            }
            else
            {
                _currentEvadeTime = evadeTime;
                Attack();
            }
        }
    }

    private void EvadePlayer()
    {
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
        Debug.Log("Shooter is attacking...");
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

    private void OnEnemyDamaged(params object[] parameters)
    {
        TakeDamage((int)parameters[0]);
    }
    
    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }
}
