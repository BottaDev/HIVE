using System;
using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using UnityEngine;

public class KamMovableManager : MonoBehaviour
{
    public static KamMovableManager i;
    [Header("Debug")]
    public bool obstacleAvoidance;
    public bool thetaStar;
    public bool drawGizmos;


    [Header("Boid Settings")]
    public float maxSpeed;
    [Range(0, 1)]
    public float maxForce;
    public LayerMask wallMask;

    [Header("Separation")]
    public float separationDistance;
    [Range(0, 1)] 
    public float separationWeight;

    [Header("Arrive")]
    public float arriveRadius;
    [Range(0, 1)]
    public float arriveWeight;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleMask;
    public float rayDistance;

    [Header("Boid List")]
    public List<KamMovable> allBoids = new List<KamMovable>();
    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Add(KamMovable boid)
    {
        if (allBoids.Contains(boid))
        {
            return;
        }

        allBoids.Add(boid);
    }

    public void Remove(KamMovable boid)
    {
        if (allBoids.Contains(boid))
        {
            allBoids.Remove(boid);
        }
    }

    public static List<KamMovable> ForAllNearbyBoids(GameObject self, float maxDistance, Action<KamMovable> action)
    {
        return KamUtilities.ForAllNearby(self, i.allBoids, maxDistance, action); ;
    }
}
