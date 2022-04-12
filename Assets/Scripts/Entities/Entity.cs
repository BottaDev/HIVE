using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    [Header("Entity Parameters")]
    public float maxHealth = 10f;
    public float baseSpeed = 5f;

    [SerializeField] private float _currentHealth;

    public float CurrentHealth { get { return _currentHealth; } protected set { _currentHealth = value; } }
    public float CurrentSpeed { get ; protected set; }

    protected virtual void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentSpeed = baseSpeed;
    }
    
    public virtual void TakeDamage(float damage) { }
    public Transform GetTransform()
    {
        return transform;
    }
}
