using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CubeAgentTestDestination : MonoBehaviour
{
    [SerializeField] private Transform _destination;

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
        Vector3 targetVector = _destination.transform.position;
        _navMeshAgent.SetDestination(targetVector);
    }

   

    
}
