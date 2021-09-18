using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AI : Entity
{
    [Header("AI Parameters")]
    [Range(0f, 3f)] public float attackRate = 1f;
    public float detectionRange = 25f;
    public float rotationSpeed = 5f;

    [Header("FOV")]
    public float angleRadius = 100f;
    public LayerMask obstacleMask;

    protected float _currentAttackRate;           
    protected Player _player;
    protected NavMeshAgent _agent;
    protected bool _playerDetected;

    protected override void Awake()
    {
        base.Awake();

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
                _playerDetected = true;   
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
    
    /// <summary>
    /// Checks if the entity is seeing the target
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    protected bool ApplyFOV(Vector3 targetPos)
    {
        Vector3 dirToTarget = targetPos - transform.position;
        
        if (dirToTarget.magnitude <= detectionRange)
        {
            if (Vector3.Angle(transform.forward, dirToTarget) < angleRadius / 2)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude,
                    obstacleMask))
                    return true;
            }
        }

        return false;
    }

    protected virtual void Attack() { }
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
