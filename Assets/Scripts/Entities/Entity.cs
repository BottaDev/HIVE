using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamagable
{
    [Header("Entity Parameters")]
    public float maxHealth = 10f;
    public float baseSpeed = 5f;
    public float damage = 1f;
    
    public float CurrentHealth { get; protected set; }
    public float CurrentSpeed { get ; private set; }

    protected virtual void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentSpeed = baseSpeed;
    }

    protected IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
    }
    
    public virtual void TakeDamage(float damage) { }
}
