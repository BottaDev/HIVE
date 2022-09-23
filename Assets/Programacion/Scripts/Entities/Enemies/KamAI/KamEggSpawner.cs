using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KamEggSpawner : Entity
{
    [Header("Egg Spawner")]
    [SerializeField] private LayerMask triggerMask;
    
    [SerializeField] private List<EggSpawn> Enemies = new List<EggSpawn>();
    
    [SerializeField] private float TriggerDelay;
    [SerializeField] private float DelayInBetweenSpawns = 0.5f;
    
    [SerializeField] private bool DieAfterSpawn;
    
    private Coroutine SpawnEnemiesCoroutine;

    public Utilities_SliderLinearProgressBar healthSlider;
    public Utilities_CanvasGroupReveal reveal;
    public GameObject deathModel;
    public GameObject normalModel;
    public GameObject activatedModel;

    [System.Serializable]
    public class EggSpawn
    {
        public GameObject enemy;
        public int amount;
        public List<BoxCollider> SpawnColliders;

        [HideInInspector]
        public int spawnedCount;
        [HideInInspector]
        public bool done;
    }

    private void Awake()
    {
        healthSlider.SetRange(0, maxHealth);
        healthSlider.SetValue(maxHealth);

        CurrentHealth = maxHealth;
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

    private IEnumerator SpawnEnemies()
    {
        Activate();
        
        yield return new WaitForSeconds(TriggerDelay);

        while (Enemies.Count(x => !x.done) != 0)
        {
            EggSpawn spawn = Enemies.Where(x => !x.done).ToList().ChooseRandom();

            Collider spawnCollider = spawn.SpawnColliders.ChooseRandom();

            Vector3 point = spawnCollider.GetRandomPositionInBounds();

            Vector3 dir = -spawnCollider.transform.up;

            Ray ray = new Ray(point, dir);

            if (spawn.spawnedCount != spawn.amount)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    GameObject obj = Instantiate(spawn.enemy, hit.point, Quaternion.identity);
                    obj.transform.rotation = Quaternion.FromToRotation(obj.transform.up, hit.normal) * obj.transform.rotation;
                    obj.transform.position += obj.transform.up * 0.1f;

                    spawn.spawnedCount++;
                    AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EnemySpawningEffect));

                    yield return new WaitForSeconds(DelayInBetweenSpawns);
                }
            }
            else
            {
                spawn.done = true;
            }
        }

        //After instantiate all enemies destroy gameObject
        if (DieAfterSpawn)
        {
            Destroy(activatedModel);
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
            int totalSpawnCount = Enemies.Aggregate(0, (acum, item) => acum + item.amount);
            
            for (int i = 0; i < totalSpawnCount; i++)
            {
                EventManager.Instance.Trigger("OnEnemyDeath");
            }

            DeathAnimation();
        }
    }

    public void Activate()
    {
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EggSpawningEffect));
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EggBoilingEffect));
        activatedModel.transform.parent = transform.parent;
        deathModel.SetActive(false);
        normalModel.SetActive(false);
        activatedModel.SetActive(true);
    }
    public void DeathAnimation()
    {
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EggSpawningEffect));
        deathModel.transform.parent = transform.parent;
        deathModel.SetActive(true);
        normalModel.SetActive(false);
        activatedModel.SetActive(false);
        gameObject.SetActive(false);
        
        Destroy(deathModel, 1f);
        Destroy(gameObject, 1f);
    }
}
