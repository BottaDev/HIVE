using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kam.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class DestructiblePlant : MonoBehaviour, IDirectionalDamageable, IHittable
{
    [Header("HP")]
    public int maxHP;
    private int currentHP;
    public Direction[] ableToBeHitFrom;
    private bool dead;
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
    
    public ParticleSphere orbParticle;
    public int minOrbAmount;
    public int maxOrbAmount;
    
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
    
    public void TakeDamageDirectional(int damage, Transform hitFrom)
    {
        if (ableToBeHitFrom.All(x => x != transform.GetDirectionTo(hitFrom))) return;

        if(dead) return;
        
        Popup.Create(hitFrom.position, damage.ToString(),KamColor.purple);
        
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
        dead = true;
        
        if (orbParticle == null)
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
        }
        
        anim.SetTrigger(deathAnim);
        onDeath.Invoke();
    }

    public void SpawnDeathParticleEffect()
    {
        if (orbParticle != null)
        {
            List<int> fullRange = new List<int>();
            for (int i = minOrbAmount; i < maxOrbAmount + 1; i++)
            {
                fullRange.Add(i);
            }

            //2, 3, 4, 5, 6, 7, 8, 9, 10
            List<int> usableInts = fullRange.Where(x => amount % x == 0).ToList();
            int amountOfOrbs = usableInts.ChooseRandom();
            
            for (int i = 0; i < amountOfOrbs; i++)
            {
                ParticleSphere orb = Instantiate(orbParticle, particleSpawningPoint.position, Quaternion.identity);
                orb.Initialize(player, type, amount / amountOfOrbs);
            }
        }
        
        if (particles != null)
        {
            GameObject obj = Instantiate(particles, particleSpawningPoint.position, Quaternion.identity);
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
