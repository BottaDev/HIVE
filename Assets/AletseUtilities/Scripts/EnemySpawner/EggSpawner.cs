using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum SpawnMethod
{
    RoundRobin,
    Random
}

public class EggSpawner : AI
{
    [Header("Spawner Enemy Settings")]
    public int numberOfEnemiesToSpawn = 5;
    public float spawnDelay = 1f;
    public SpawnMethod enemySpawnMethod = SpawnMethod.RoundRobin;
    public List<AI> enemyPrefab = new List<AI>();
    
    
    private Dictionary<int, ObjectPool> enemyObjectPools = new Dictionary<int, ObjectPool>();
    private NavMeshTriangulation _triangulation; 

    protected override void Awake()
    {
        _fov = GetComponent<FOV>();
        _agent = GetComponent<NavMeshAgent>();
        _player = FindObjectOfType<Player>();
        
        _currentAttackRate = 0;

        if (isAEnemySpawner == false)
        {
            return;
        }
        
        for (int i = 0; i < enemyPrefab.Count; i++)
        {
            //enemyObjectPools.Add(i, ObjectPool.CreateInstance(enemyPrefab[i], numberOfEnemiesToSpawn));
        }

        if (_playerDetected)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    private void Start()
    {
        _triangulation = NavMesh.CalculateTriangulation();
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        int spawnedEnemies = 0;

        while (spawnedEnemies < numberOfEnemiesToSpawn)
        {
            if (enemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(spawnedEnemies);
            }
            else if (enemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }

            spawnedEnemies++;
            
            yield return wait;
        }
    }

    private void SpawnRoundRobinEnemy(int spawnedEnemies)
    {
        int spawnIndex = spawnedEnemies % enemyPrefab.Count;

        DoSpawnEnemy(spawnIndex);
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, enemyPrefab.Count));
    }
    
    private void DoSpawnEnemy(int spawnIndex)
    {
        PoolableObject poolableObject = enemyObjectPools[spawnIndex].GetObject();

        if (poolableObject != null)
        {
            AI aiEnemy = poolableObject.GetComponent<AI>();
            
            int vertexIndex = Random.Range(0, _triangulation.vertices.Length);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(_triangulation.vertices[vertexIndex], out hit, 2f, -1))
            {
                aiEnemy._agent.Warp(hit.position);
                aiEnemy._agent.enabled = true;
                aiEnemy.StartChasing();
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use {_triangulation.vertices[vertexIndex]}");
            }
        }
        else
        {
            Debug.LogError($"Unable to fetch enemy of type {spawnIndex} from object pool. Out of objects?");
        }
    }
}
