using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EggSpawner : Entity
{
    [Header("Egg Spawner")]
    [SerializeField] private LayerMask triggerMask;
    public List<BoxCollider> SpawnCollider;
    [SerializeField] private EnemySpawner EnemySpawner;
    [SerializeField] private List<AI> Enemies = new List<AI>();
    [SerializeField] private EnemySpawner.SpawnMethod SpawnMethod = EnemySpawner.SpawnMethod.Random;
    [SerializeField] private int SpawnCount = 10;
    [SerializeField] private float DelayBeforeSpawn;
    [SerializeField] private float SpawnDelay = 0.5f;
    [SerializeField] private bool DieAfterSpawn;
    [SerializeField] private bool UpsideDown;
    private Coroutine SpawnEnemiesCoroutine;

    public Utilities_SliderLinearProgressBar healthSlider;
    public Utilities_CanvasGroupReveal reveal;
    public GameObject deathModel;
    public GameObject normalModel;
    public enum Side
    {
        down, up, front, back, left, right
    }

    private void Awake()
    {
        EnemySpawner = FindObjectOfType<EnemySpawner>();
        healthSlider.SetRange(0, maxHealth);
        healthSlider.SetValue(maxHealth);

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
        if (triggerMask.CheckLayer(other.gameObject.layer))
        {
            if (SpawnEnemiesCoroutine == null)
            {
                SpawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());
            }
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

        DeathAnimation();
        
        yield return waitBeforeSpawn;

        for (int i = 0; i < SpawnCount; i++)
        {
            int index = 0;
            switch (SpawnMethod)
            {
                case EnemySpawner.SpawnMethod.RoundRobin:
                    index = i % Enemies.Count;
                    break;

                case EnemySpawner.SpawnMethod.Random:
                    index = Random.Range(0, Enemies.Count);
                    break;
            }

            int spawnIndex = EnemySpawner.Enemies.FindIndex((enemy) => enemy.Equals(Enemies[index]));

            bool result = false;
            int maxIterations = 100;
            while (!result && maxIterations >= 0)
            {
                maxIterations--;
                Vector3 position = GetRandomPositionInBounds();
                result = EnemySpawner.DoSpawnEnemy(spawnIndex, position);
            }

            AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EnemySpawningEffect));
            yield return wait;
        }

        //After instantiate all enemies destroy gameObject
        if (DieAfterSpawn)
        {
            Destroy(deathModel);
            Destroy(gameObject);
        }
    }

    public override void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        reveal.RevealTemporary();
        healthSlider.SetValue(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            for (int i = 0; i < SpawnCount; i++)
            {
                EventManager.Instance.Trigger("OnEnemyDeath");
            }

            DeathAnimation();
        }
    }

    public void DeathAnimation()
    {
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EggSpawningEffect));
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EggBoilingEffect));
        deathModel.transform.parent = transform.parent;
        deathModel.SetActive(true);
        normalModel.SetActive(false);
    }

}
