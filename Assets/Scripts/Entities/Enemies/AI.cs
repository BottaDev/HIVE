using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(FOV))]
public abstract class AI : Entity
{
    [Header("AI Parameters")]
    [Range(0f, 3f)] public float attackRate = 1f;
    public float detectionRange = 25f;
    public float rotationSpeed = 5f;

    protected float _currentAttackRate;           
    protected Player _player;
    protected NavMeshAgent _agent;
    protected FOV _fov;
    protected bool _playerDetected;

    protected override void Awake()
    {
        base.Awake();

        _fov = GetComponent<FOV>();
        _agent = GetComponent<NavMeshAgent>();
        _player = FindObjectOfType<Player>();

        _currentAttackRate = 0;
    }

    protected virtual void Update()
    {
        CheckPlayerDistance();

        // Update the agent speed all the time...
        _agent.speed = CurrentSpeed;
    }

    private void CheckPlayerDistance()
    {
        if (!_playerDetected)
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance <= detectionRange)
                DetectPlayer();   
        }
    }

    private void DetectPlayer()
    {
        _playerDetected = true;

        // Warn other enemies
        List<AI> nearbyEnemies = FindObjectsOfType<AI>()
            .Where(x => (transform.position - x.transform.position).magnitude <= detectionRange)
            .ToList();
        
        foreach (AI ai in nearbyEnemies)
        {
            ai._playerDetected = true;
        }
    }

    protected void MoveToPosition(Vector3 position)
    {
        _agent.destination = position;
    }
    
    protected void RotateTowards (Vector3 target) 
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    protected virtual void Attack() { }
    
    public override void TakeDamage(float damage)
    {
        if (!_playerDetected)
            DetectPlayer();
        
        CurrentHealth -= damage;
        
        if(CurrentHealth <= 0)
            KillAI();
    }

    private void KillAI()
    {
        EventManager.Instance.Trigger("OnEnemyDeath");

        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
