using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBUild : MonoBehaviour
{
    [SerializeField] private NavMeshSurface[] _navMeshSurfaces;
    // Start is called before the first frame update
    void Awake()
    {
        //FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        for (int i = 0; i < _navMeshSurfaces.Length; i++)
        {
            _navMeshSurfaces[i].BuildNavMesh();
        }
    }

    private void Update()
    {
        
    }
}
