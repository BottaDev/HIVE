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
    public float CurrentSpeed { get ; protected set; }

    protected virtual void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentSpeed = baseSpeed;
    }
    
    public virtual void TakeDamage(float damage) { }
}
