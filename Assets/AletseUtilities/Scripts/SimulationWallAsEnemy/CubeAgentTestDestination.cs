using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class CubeAgentTestDestination : Entity
{
    public Transform player;

    private NavMeshAgent _navMeshAgent;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to" + gameObject.name);
            return;
        }
    
    }

    private void Update()
    {
        _navMeshAgent.SetDestination(player.position);
        _navMeshAgent.speed = baseSpeed;
    }


    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
            Destroy(gameObject);
    }
}
