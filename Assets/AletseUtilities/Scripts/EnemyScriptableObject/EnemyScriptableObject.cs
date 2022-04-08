using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ScritableObject that holds the BASE STATS for an enemy. These can then be modified at object creation time to buff up enemies.
/// and to reset their stats if they died or were modified at runtime.
/// </summary>

[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScritableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    public Enemy prefab;
    public AttackScriptableObject attackConfiguration;
    
    [Header("Enemy Stats")] 
    public int health = 100;

    [Header("Movement Stats")] 
    public EnemyState defaultState;        
    public float idleLocationRadius = 4f;
    public float idleMoveSpeedMultiplier = 0.5f;
    [Range(2, 10)] public int waypoints = 4;
    public float lineOfSightRange = 6f;
    public float fieldOfView = 90f;
    
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

    public void SetupEnemy(Enemy enemy)
    {
        enemy.agent.acceleration = acceleration;
        enemy.agent.angularSpeed = angularSpeed;
        enemy.agent.areaMask = areaMask;
        enemy.agent.avoidancePriority = avoidancePriority;
        enemy.agent.baseOffset = baseOffset;
        enemy.agent.height = height;
        enemy.agent.obstacleAvoidanceType = obstacleAvoidanceType;
        enemy.agent.radius = radius;
        enemy.agent.speed = speed;
        enemy.agent.stoppingDistance = stoppingDistance;

        enemy.movement.updateRate = aIUpdateInterval;
        enemy.movement.DefaultState = defaultState;
        enemy.movement.idleMoveSpeedMultiplier = idleMoveSpeedMultiplier;
        enemy.movement.idleLocationRadius = idleLocationRadius;
        enemy.movement.waypoints = new Vector3[waypoints];
        enemy.movement.lineOfSightChecker.fieldOfView = fieldOfView;
        enemy.movement.lineOfSightChecker.collider.radius = lineOfSightRange;
        enemy.movement.lineOfSightChecker.lineOfSightLayers = attackConfiguration.LineOfSightLayers;
        
        enemy.health = health;
        
        attackConfiguration.SetupEnemy(enemy);
    }
}
