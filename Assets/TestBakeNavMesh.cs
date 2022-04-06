using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestBakeNavMesh : MonoBehaviour
{
    public NavMeshSurface[] NavMeshSurfaces;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < NavMeshSurfaces.Length; i++)
        {
            NavMeshSurfaces[i].BuildNavMesh();
            Debug.Log(NavMeshSurfaces[i]);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
