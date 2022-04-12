using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ScriptableObject that holds the BASE STATS for an enemy. These can then be modified at object creation time to buff up enemies
/// and to reset their stats if they died or were modified at runtime.
/// </summary>
[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScriptableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    public EnemyIA Prefab;
    public AttackScriptableObject AttackConfiguration;

    // Enemy Stats
    public int Health = 100;

    // Movement Stats
    public EnemyState DefaultState;
    public float IdleLocationRadius = 4f;
    public float IdleMovespeedMultiplier = 0.5f;
    [Range(2, 10)]
    public int Waypoints = 4;
    public float LineOfSightRange = 6f;
    public float FieldOfView = 90f;

    // NavMeshAgent Configs
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 8;
    public float AngularSpeed = 120;
    // -1 means everything
    public int AreaMask = -1;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2f;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float Speed = 3f;
    public float StoppingDistance = 0.5f;

    public EnemyScriptableObject ScaleUpForLevel(ScalingScriptableObject Scaling, int Level)
    {
        EnemyScriptableObject scaledUpEnemy = CreateInstance<EnemyScriptableObject>();

        scaledUpEnemy.name = name;
        scaledUpEnemy.Prefab = Prefab;

        scaledUpEnemy.AttackConfiguration = AttackConfiguration.ScaleUpForLevel(Scaling, Level);

        scaledUpEnemy.Health = Mathf.FloorToInt(Health * Scaling.HealthCurve.Evaluate(Level));

        scaledUpEnemy.DefaultState = DefaultState;
        scaledUpEnemy.IdleLocationRadius = IdleLocationRadius;
        scaledUpEnemy.IdleMovespeedMultiplier = IdleMovespeedMultiplier;
        scaledUpEnemy.Waypoints = Waypoints;
        scaledUpEnemy.LineOfSightRange = LineOfSightRange;
        scaledUpEnemy.FieldOfView = FieldOfView;

        scaledUpEnemy.AIUpdateInterval = AIUpdateInterval;
        scaledUpEnemy.Acceleration = Acceleration;
        scaledUpEnemy.AngularSpeed = AngularSpeed;

        scaledUpEnemy.AreaMask = AreaMask;
        scaledUpEnemy.AvoidancePriority = AvoidancePriority;

        scaledUpEnemy.BaseOffset = BaseOffset;
        scaledUpEnemy.Height = Height;
        scaledUpEnemy.ObstacleAvoidanceType = ObstacleAvoidanceType;
        scaledUpEnemy.Radius = Radius;
        scaledUpEnemy.Speed = Speed * Scaling.SpeedCurve.Evaluate(Level);
        scaledUpEnemy.StoppingDistance = StoppingDistance;

        return scaledUpEnemy;
    }

    public void SetupEnemy(EnemyIA enemyIa)
    {
        enemyIa.Agent.acceleration = Acceleration;
        enemyIa.Agent.angularSpeed = AngularSpeed;
        enemyIa.Agent.areaMask = AreaMask;
        enemyIa.Agent.avoidancePriority = AvoidancePriority;
        enemyIa.Agent.baseOffset = BaseOffset;
        enemyIa.Agent.height = Height;
        enemyIa.Agent.obstacleAvoidanceType = ObstacleAvoidanceType;
        enemyIa.Agent.radius = Radius;
        enemyIa.Agent.speed = Speed;
        enemyIa.Agent.stoppingDistance = StoppingDistance;

        enemyIa.Movement.UpdateRate = AIUpdateInterval;
        enemyIa.Movement.DefaultState = DefaultState;
        enemyIa.Movement.IdleMovespeedMultiplier = IdleMovespeedMultiplier;
        enemyIa.Movement.IdleLocationRadius = IdleLocationRadius;
        enemyIa.Movement.Waypoints = new Vector3[Waypoints];
        enemyIa.Movement.LineOfSightChecker.FieldOfView = FieldOfView;
        enemyIa.Movement.LineOfSightChecker.Collider.radius = LineOfSightRange;
        enemyIa.Movement.LineOfSightChecker.LineOfSightLayers = AttackConfiguration.LineOfSightLayers;

        enemyIa.Health = Health;

        AttackConfiguration.SetupEnemy(enemyIa);
    }
}
