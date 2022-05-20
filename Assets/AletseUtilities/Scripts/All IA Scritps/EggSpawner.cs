using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EggSpawner : Entity
{
    [Header("Egg Spawner")]
    [SerializeField] private List<BoxCollider> SpawnCollider;
    [SerializeField] private EnemySpawner EnemySpawner;
    [SerializeField] private List<AI> Enemies = new List<AI>();
    [SerializeField] private EnemySpawner.SpawnMethod SpawnMethod = EnemySpawner.SpawnMethod.Random;
    [SerializeField] private int SpawnCount = 10;
    [SerializeField] private float DelayBeforeSpawn;
    [SerializeField] private float SpawnDelay = 0.5f;
    [SerializeField] private bool DieAfterSpawn;
    [SerializeField] private bool UpsideDown;
    private Coroutine SpawnEnemiesCoroutine;
    
    public Slider healthSlider;
    public enum Side
    {
        down, up, front, back, left, right
    }
    
    private void Awake()
    {
        EnemySpawner = FindObjectOfType<EnemySpawner>();
        healthSlider = GetComponentInChildren<Slider>();
        
        CurrentHealth = maxHealth;
        
        if (SpawnCollider != null)
        {
            if (SpawnCollider.Count == 0)
            {
                SpawnCollider.Add(GetComponent<BoxCollider>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var bullet = other.GetComponent<Bullet>();
    
        if (bullet != null)
        {
            Destroy(gameObject);
        }
        
        if (SpawnEnemiesCoroutine == null && other != bullet)
        {
            SpawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    private Vector3 GetRandomPositionInBounds()
    {
        Bounds _bounds = SpawnCollider[Random.Range(0, SpawnCollider.Count)].bounds;
        return new Vector3(Random.Range(_bounds.min.x, _bounds.max.x), UpsideDown ? _bounds.max.y : _bounds.min.y,
            Random.Range(_bounds.min.z, _bounds.max.z));
    }
    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds wait = new WaitForSeconds(SpawnDelay);

        WaitForSeconds waitBeforeSpawn = new WaitForSeconds(DelayBeforeSpawn);

        yield return waitBeforeSpawn;
        
        for (int i = 0; i < SpawnCount; i++)
        {
            if (SpawnMethod == EnemySpawner.SpawnMethod.RoundRobin)
            {
                EnemySpawner.DoSpawnEnemy(EnemySpawner.Enemies.FindIndex((enemy) => enemy.Equals(Enemies[i % Enemies.Count])),
                    GetRandomPositionInBounds());
            }
            else if (SpawnMethod == EnemySpawner.SpawnMethod.Random)
            {
                int index = Random.Range(0, Enemies.Count);
                EnemySpawner.DoSpawnEnemy(EnemySpawner.Enemies.FindIndex((enemy) => enemy.Equals(Enemies[index])),
                GetRandomPositionInBounds());
            }
            
            yield return wait;
        }

        //After instantiate all enemies destroy gameObject
        if (DieAfterSpawn)
        {
            Destroy(gameObject);
        }
    }

    public override void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        healthSlider.value = CurrentHealth / 10f;
        
        if(CurrentHealth <= 0)
        {
            for (int i = 0; i < SpawnCount; i++)
            {
                EventManager.Instance.Trigger("OnEnemyDeath");
            }

            Destroy(gameObject);
        }
    }
}
