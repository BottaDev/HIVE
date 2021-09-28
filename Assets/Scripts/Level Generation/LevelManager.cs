using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    private levelGen _levelGen;

    private void Awake()
    {
        _levelGen = GetComponent<levelGen>();
    }

    private void Start()
    {
        _levelGen.StartGeneration();
        
        // Bake map navmesh
        FindObjectOfType<NavMeshSurface>().BuildNavMesh();
    }
}
