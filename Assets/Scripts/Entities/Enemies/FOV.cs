using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    public float angleRadius = 100f;
    public LayerMask obstacleMask;
    
    private AI _ai;
    
    private void Awake()
    {
        _ai = GetComponent<AI>();
    }

    /// <summary>
    /// Checks if there is an obstacle between the enemy and the target
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public bool CheckMiddleObstacle(Vector3 targetPos)
    {
        Vector3 dirToTarget = targetPos - transform.position;
        
        if (dirToTarget.magnitude <= _ai.detectionRange)
        {
            if (!Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude,
                obstacleMask))
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// Checks if the entity is seeing the target
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public bool ApplyFOV(Vector3 targetPos)
    {
        Vector3 dirToTarget = targetPos - transform.position;
        
        if (dirToTarget.magnitude <= _ai.detectionRange)
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
}
