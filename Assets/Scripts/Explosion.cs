using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float duration = 0.1f;
    private float _damage;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    public void SetDamage(float dmg)
    {
        _damage = dmg;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Entity ent = other.gameObject.GetComponent<Entity>(); 
        if (ent != null)
            ent.TakeDamage(_damage);
    }
}
