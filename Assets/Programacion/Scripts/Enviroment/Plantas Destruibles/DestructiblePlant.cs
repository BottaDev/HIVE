using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class DestructiblePlant : MonoBehaviour, IDamageable, IHittable
{
    [Header("HP")]
    public int maxHP;
    private int currentHP;
    public Utilities_ProgressBar healthSlider;
    public Utilities_CanvasGroupReveal reveal;

    [Header("Plant Type")]
    public PlantType type;
    public int amount;
    private Player player;
    
    public enum PlantType
    {
        Energy, HP
    }
    
    [Header("Directional Animations")]
    public Animator anim;
    public string backHit;
    public string frontHit;
    public string leftHit;
    public string rightHit;
    
    [Header("Death Animation")]
    public string deathAnim;
    public Transform particleSpawningPoint;
    public GameObject particles;
    public float destroyAfter;
    public UnityEvent onDeath;

    
    
    private void Start()
    {
        currentHP = maxHP;
        healthSlider.SetRange(0,maxHP);
        healthSlider.SetValue(maxHP);
        
        EventManager.Instance.Subscribe("SendPlayerReference", GetPlayerReference);
        EventManager.Instance.Trigger("NeedsPlayerReference");
        EventManager.Instance.Unsubscribe("SendPlayerReference", GetPlayerReference);
    }
    
    private void GetPlayerReference(params object[] p)
    {
        player = (Player)p[0];
    }
    
    public void TakeDamage(int damage)
    {

        currentHP -= damage;

        if (healthSlider != null)
        {
            reveal.RevealTemporary();
            healthSlider.SetValue(currentHP);
        }

        if(currentHP <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        switch (type)
        {
            case PlantType.Energy:
                player.energy.AddEnergy(amount);
                break;
            
            case PlantType.HP:
                player.Heal(amount);
                break;
        }
        anim.SetTrigger(deathAnim);
        onDeath.Invoke();
    }

    public void SpawnDeathParticleEffect()
    {
        if (particles != null)
        {
            GameObject obj = Instantiate(particles, particleSpawningPoint.position, particleSpawningPoint.rotation, null);
            Destroy(obj, destroyAfter);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Hit(Transform origin)
    {
        switch (transform.GetDirectionTo(origin))
        {
            case Direction.Back:
                anim.SetTrigger(backHit);
                break;

            case Direction.Front:
                anim.SetTrigger(frontHit);
                break;
            
            case Direction.Left:
                anim.SetTrigger(leftHit);
                break;
            
            case Direction.Right:
                anim.SetTrigger(rightHit);
                break;
        }
    }
}
