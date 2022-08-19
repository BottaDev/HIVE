using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizrarSpawnPoint : MonoBehaviour
{
    public ShootingPoint shootingPoint;
    public BoxCollider collider;
    void Start()
    {
        shootingPoint.action = delegate(GameObject o)
        {
            o.GetComponentInChildren<EggSpawner>().SpawnCollider = new List<BoxCollider>() { collider }; };
    }
}
