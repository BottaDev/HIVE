using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public abstract class Entity : PoolableObject, IDamageable
{
    [Header("Entity Parameters")]
    public int maxHealth = 10;
    public float baseSpeed = 5f;

    [SerializeField] private int _currentHealth;

    public int CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            int newHp = value;
            if (newHp < 0)
            {
                _currentHealth = 0;
            }
            else if (newHp > maxHealth)
            {
                _currentHealth = maxHealth;
            }
            else
            {
                _currentHealth = newHp;
            }
        }
    }
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
