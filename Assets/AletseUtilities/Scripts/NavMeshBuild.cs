using UnityEngine;
using UnityEngine.AI;


public class NavMeshBuild : MonoBehaviour
{
    public NavMeshSurface[] navMeshSurfaces;
    
    // Start is called before the first frame update
    void Awake()
    {
       ////FindObjectOfType<NavMeshSurface>().BuildNavMesh();
       //for (int i = 0; i < navMeshSurfaces.Length; i++)
       //{
       //    navMeshSurfaces[i].BuildNavMesh();
       //}
       //Debug.Log("terminé de bakear");
    }
}
