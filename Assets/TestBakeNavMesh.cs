using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBakeNavMesh : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        FindObjectOfType<NavMeshSurface>().BuildNavMesh();
    }
    
}
