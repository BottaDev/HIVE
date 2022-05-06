using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public Transform Player;
    public int NumberOfEnemiesToSpawn = 5;
    public float SpawnDelay = 1f;
    public List<AI> Enemies = new List<AI>();
    public ScalingScriptableObject Scaling;
    public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;
    public bool ContinuousSpawning;
    [Space]
    [Header("Read At Runtime")]
    [SerializeField]
    private int Level = 0;
    //[SerializeField]
    //private List<EnemyScriptableObject> ScaledEnemies = new List<EnemyScriptableObject>();

    private int EnemiesAlive = 0;
    private int SpawnedEnemies = 0;
    private int InitialEnemiesToSpawn;
    private float InitialSpawnDelay;


    private NavMeshTriangulation Triangulation;
    private Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();

    private void Awake()
    {
        for (var i = 0; i < Enemies.Count; i++)
        {
            EnemyObjectPools.Add(i, ObjectPool.CreateInstance(Enemies[i], NumberOfEnemiesToSpawn));
        }

        InitialEnemiesToSpawn = NumberOfEnemiesToSpawn;
        InitialSpawnDelay = SpawnDelay;
    }

    private void Start()
    {
        Triangulation = NavMesh.CalculateTriangulation();

        for (int i = 0; i < Enemies.Count; i++)
        {
            //ScaledEnemies.Add(Enemies[i].ScaleUpForLevel(Scaling, 0));
        }

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        Level++;
        SpawnedEnemies = 0;
        EnemiesAlive = 0;
        for (int i = 0; i < Enemies.Count; i++)
        {
            //ScaledEnemies[i] = Enemies[i].ScaleUpForLevel(Scaling, Level);
        }

        WaitForSeconds Wait = new WaitForSeconds(SpawnDelay);

        while (SpawnedEnemies < NumberOfEnemiesToSpawn)
        {
            if (EnemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(SpawnedEnemies);
            }
            else if (EnemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }

            SpawnedEnemies++;

            yield return Wait;
        }

        if (ContinuousSpawning)
        {
            ScaleUpSpawns();
            StartCoroutine(SpawnEnemies());
        }
    }

    private void SpawnRoundRobinEnemy(int spawnedEnemies)
    {
        var SpawnIndex = spawnedEnemies % Enemies.Count;
        DoSpawnEnemy(SpawnIndex, ChooseRandomPositionOnNavMesh());
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, Enemies.Count), ChooseRandomPositionOnNavMesh());
    }

    private Vector3 ChooseRandomPositionOnNavMesh()
    {
        int VertexIndex = Random.Range(0, Triangulation.vertices.Length);
        return Triangulation.vertices[VertexIndex];
    }

    public void DoSpawnEnemy(int SpawnIndex, Vector3 SpawnPosition)
    {
        var poolableObject = EnemyObjectPools[SpawnIndex].GetObject();

        if (poolableObject != null)
        {
            AI enemyIa = poolableObject.GetComponent<AI>();
            
            if (NavMesh.SamplePosition(SpawnPosition, out var Hit, 2f, -1))
            {
                enemyIa._agent.Warp(Hit.position);
                // enemy needs to get enabled and start chasing now.
                enemyIa.Player = Player;
                enemyIa.Triangulation = Triangulation;
                enemyIa._agent.enabled = true;
                EnemiesAlive++;
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {SpawnPosition}");
            }
        }
        else
        {
            Debug.LogError($"Unable to fetch enemy of type {SpawnIndex} from object pool. Out of objects?");
        }
    }

    private void ScaleUpSpawns()
    {
        NumberOfEnemiesToSpawn = Mathf.FloorToInt(InitialEnemiesToSpawn * Scaling.SpawnCountCurve.Evaluate(Level + 1));
        SpawnDelay = InitialSpawnDelay * Scaling.SpawnRateCurve.Evaluate(Level + 1);
    }

    private void HandleEnemyDeath(EnemyIA enemyIa)
    {
        EnemiesAlive--;

        if (EnemiesAlive == 0 && SpawnedEnemies == NumberOfEnemiesToSpawn)
        {
            ScaleUpSpawns();
            StartCoroutine(SpawnEnemies());
        }
    }


    public enum SpawnMethod
    {
        RoundRobin,
        Random
        // Other spawn methods can be added here
    }

    public void DoSpawnEnemy(int spawnIndex)
    {
        var poolableObject = EnemyObjectPools[spawnIndex].GetObject();

        if (poolableObject != null)
        {
            var enemy = poolableObject.GetComponent<AI>();

            var VertexIndex = Random.Range(0, Triangulation.vertices.Length);

            if (NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out var hit, 2f, -1))
            {
                enemy._agent.Warp(hit.position);
                enemy.Player = Player;
                enemy.Triangulation = Triangulation;
                enemy._agent.enabled = true;
            }
            else
            {
                //Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use"(Triangulation.vertices[VertexIndex]));
            }
        }
        else
        {
            Debug.LogError($"Unable to fetch enemy of type {spawnIndex} from object pool. Out of objects?");
        }
    }
}
