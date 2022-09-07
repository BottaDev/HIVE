using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Node actualNode;
    public List<Node> _neighbors = new List<Node>();
    public int cost = 1;
    public float radius;
    public float maxDistance;
    public LayerMask layerMaskHittable;

    public List<Node> GetNeighbors()    
    {
        return _neighbors;
    }

    private void Start()
    {
        actualNode = this;

        
    }

    private void Update()
    {
     
    }

    private void OnDrawGizmos()
    {
        foreach (var item in _neighbors)
        {
            Gizmos.DrawLine(transform.position, item.transform.position);
        }
    }
}
