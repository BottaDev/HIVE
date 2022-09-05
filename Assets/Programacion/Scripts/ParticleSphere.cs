using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleSphere : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    float waitBeforeEffect, minStartForce, maxStartForce, speed, acceleration;
    public ParticleSystem effectParticles;

    private DestructiblePlant.PlantType type;
    private float effect;

    Player player;
    private Action onUpdate = delegate { };

    public void Initialize(Player player, DestructiblePlant.PlantType type, float effect)
    {
        this.type = type;
        rb = GetComponent<Rigidbody>();
        this.effect = effect;
        this.player = player;
        var startDir = new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5),
            UnityEngine.Random.Range(-5, 5)).normalized;
        rb.AddForce(startDir * Random.Range(minStartForce, maxStartForce), ForceMode.Impulse);

        onUpdate += () =>
        {
            transform.LookAt(Camera.main.transform);

            speed += acceleration * Time.deltaTime;

            var dir = (this.player.transform.position - transform.position).normalized;
            rb.AddForce(dir * speed);
        };
    }

    private void Update()
    {
        onUpdate.Invoke();
    }

    private bool triggeredEffect;
    public void ApplyEffect()
    {
        if (triggeredEffect) return;
        
        StartCoroutine(ApplyEffectCoroutine());
    }

    IEnumerator ApplyEffectCoroutine()
    {
        triggeredEffect = true;

        yield return new WaitForSeconds(waitBeforeEffect);

        if(effectParticles != null)
        {
            Instantiate(effectParticles, transform.position, Quaternion.identity);
        }
        

        switch (type)
        {
            case DestructiblePlant.PlantType.Energy:
                player.energy.AddEnergy(effect);
                break;

            case DestructiblePlant.PlantType.HP:
                player.Heal((int) effect);
                break;
        }
        
        Destroy(gameObject);
    }

}