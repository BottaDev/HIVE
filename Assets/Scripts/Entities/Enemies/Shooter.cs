using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;

public class Shooter : AI
{
    [Header("Shooter Parameters")]
    public float attackDistance = 20f;
    public float evadeDistance = 12f;
    public float evadeTime = 1.5f;

    [Header("Shooter Animations")]
    public string attackTrigger;
    
    [Header("Objects")]
    public GameObject bulletPrefab;
    public Transform spawnPos;
    
    private float _currentEvadeTime;

    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.Subscribe(EventManager.Events.OnEnemyDamaged, OnEnemyDamaged);
        
        _currentEvadeTime = evadeTime;
    }

    protected override void Update()
    {
        base.Update();
        if (dying) return;


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
            if (distance <= evadeDistance)
            {
                // If is jumping, follow player
                if (_agent.isOnOffMeshLink)
                {
                    ChasePlayer();
                }
                else
                {
                    _currentEvadeTime -= Time.deltaTime;
                    if (_fov.CheckMiddleObstacle(_player.transform.position))
                    {
                        if ( _currentEvadeTime > 0)
                            EvadePlayer();
                        else
                            Attack();    
                    }
                    else
                    {
                        ChasePlayer();
                    }   
                }
            }
            else
            {
                // If is jumping, follow player
                if (_agent.isOnOffMeshLink)
                {
                    ChasePlayer();
                }
                else
                {
                    _currentEvadeTime = evadeTime;
                    // In Attack distance...
                    if (_fov.ApplyFOV(_player.transform.position))
                        Attack();
                    else
                        ChasePlayer();   
                }
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
        if (_currentAttackRate <= 0)
        {
            AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EnemyShot));
            anim.SetTrigger(attackTrigger);
            
            GameObject bullet = Instantiate(bulletPrefab, spawnPos.position, Quaternion.identity);
            bullet.transform.LookAt(_player.transform.position);
            
            _currentAttackRate = attackRate;
            
        }
        else
        {
            _currentAttackRate -= Time.deltaTime;
        }
        
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
}
