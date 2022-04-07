using UnityEngine;
using UnityEngine.AI;

public class Enemy : PoolableObject
{
    public NavMeshAgent agent;
    public EnemyScriptableObject enemyScriptableObject;

    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration();
    }

    public virtual void SetupAgentFromConfiguration()
    {
        agent.acceleration = enemyScriptableObject.acceleration;
        agent.angularSpeed = enemyScriptableObject.angularSpeed;
        agent.areaMask = enemyScriptableObject.areaMask;
        agent.avoidancePriority = enemyScriptableObject.avoidancePriority;
        agent.baseOffset = enemyScriptableObject.baseOffset;
        agent.height = enemyScriptableObject.height;
        agent.obstacleAvoidanceType = enemyScriptableObject.obstacleAvoidanceType;
        agent.radius = enemyScriptableObject.radius;
        agent.speed = enemyScriptableObject.speed;
        agent.stoppingDistance = enemyScriptableObject.stoppingDistance;
    }
}
