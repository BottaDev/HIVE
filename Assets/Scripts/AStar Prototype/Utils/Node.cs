using System.Collections.Generic;
using UnityEngine;

//public enum DirectionToConnect
//{
//    Forward,
//    Right
//}

public class Node : MonoBehaviour
{
    private Node actualNode;
    [SerializeField]
    private List<Node> _neighbors = new List<Node>();
    public int cost = 1;
    //public float radius;
    //public LayerMask layerMaskHittable;
    public MeshRenderer meshRend;

    public List<Node> GetNeighbors()    
    {
        return _neighbors;
    }

    private void OnDrawGizmos()
    {
        //Connect all neighbors each other on Gizmos
        foreach (var item in _neighbors)
        {
            Gizmos.DrawLine(transform.position, item.transform.position);
        }
    }
}
