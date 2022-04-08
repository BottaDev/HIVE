using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ScritableObject that holds the BASE STATS for an enemy. These can then be modified at object creation time to buff up enemies.
/// and to reset their stats if they died or were modified at runtime.
/// </summary>

[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScritableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Enemy Stats")] 
    public int health = 100;
    public float attackDelay = 1f;
    public int damage = 5;
    public float attackRadius = 1.5f;

    [Header("NavMeshAgent Configuration")]
    public float aIUpdateInterval = 0.1f;
    public float acceleration = 8;
    public float angularSpeed = 120f;
    //-1 means everything
    public int areaMask = -1;
    public int avoidancePriority = 50;
    public float baseOffset = 0;
    public float height = 2f;
    public ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float radius = 0.5f;
    public float speed = 3f;
    public float stoppingDistance = 0.5f;
}
