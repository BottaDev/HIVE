using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum SpawnMethod
{
    RoundRobin,
    Random
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Enemy Settings")] 
    public Transform player;
    public int numberOfEnemiesToSpawn = 5;
    public float spawnDelay = 1f;
    public SpawnMethod enemySpawnMethod = SpawnMethod.RoundRobin;
    public List<EnemyScriptableObject> enemies = new List<EnemyScriptableObject>();
    
    
    private Dictionary<int, ObjectPool> enemyObjectPools = new Dictionary<int, ObjectPool>();
    private NavMeshTriangulation _triangulation; 

    private void Awake()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyObjectPools.Add(i, ObjectPool.CreateInstance(enemies[i].prefab, numberOfEnemiesToSpawn));
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
        int spawnIndex = spawnedEnemies % enemies.Count;

        DoSpawnEnemy(spawnIndex);
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, enemies.Count));
    }
    
    private void DoSpawnEnemy(int spawnIndex)
    {
        PoolableObject poolableObject = enemyObjectPools[spawnIndex].GetObject();

        if (poolableObject != null)
        {
            Enemy enemy = poolableObject.GetComponent<Enemy>();
            enemies[spawnIndex].SetupEnemy(enemy);
            
            int vertexIndex = Random.Range(0, _triangulation.vertices.Length);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(_triangulation.vertices[vertexIndex], out hit, 2f, -1))
            {
                enemy.agent.Warp(hit.position);
                enemy.movement.player = player;
                enemy.movement.triangulation = _triangulation;
                enemy.agent.enabled = true;
                enemy.movement.Spawn();
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
