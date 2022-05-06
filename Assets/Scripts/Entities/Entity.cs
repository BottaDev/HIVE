using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : PoolableObject, IDamageable
{
    [Header("Entity Parameters")]
    public int maxHealth = 10;
    public float baseSpeed = 5f;

    [SerializeField] private int _currentHealth;

    public int CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }
    public float CurrentSpeed { get ; protected set; }

    protected virtual void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentSpeed = baseSpeed;
    }
    
    public virtual void TakeDamage(int damage) { }
    public Transform GetTransform()
    {
        return transform;
    }
}
