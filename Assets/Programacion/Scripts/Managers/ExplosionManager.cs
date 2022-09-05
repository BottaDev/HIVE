using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager i;
    
    public Explosion prefab;
    
    public Explosion NewExplosion(Vector3 point, float explosionRadius, LayerMask mask)
    {
        return Instantiate(prefab).Initialize(point, explosionRadius, mask);
    }

    private void Awake()
    {
        i = this;
    }
}
