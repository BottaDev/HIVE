using System.Collections.Generic;
using UnityEngine;

public enum RayDirections
{
    Forward,
    NegativeForward,
    Right,
    NegativeRight
}


public class Node : MonoBehaviour
{
    [SerializeField]
    private List<Node> _neighbors = new List<Node>();
    public int cost = 1;
    //public float radius;
    //public LayerMask layerMaskHittable;
    public RayDirections rayDirection;
    private Vector3 direction;
    public MeshRenderer meshRend;
    public LayerMask nodeMask;
    public bool NeedToConnect;

    private void Awake()
    {
        if (!NeedToConnect) return;

        ConnectNodesByRaycast();
        
    }

    private void ConnectNodesByRaycast()
    {
        RaycastHit hit;

        switch (rayDirection)
        {
            case RayDirections.Forward:
                direction = Vector3.forward;
                break;
            case RayDirections.NegativeForward:
                direction = -Vector3.forward;
                break;
            case RayDirections.Right:
                direction = Vector3.right;
                break;
            case RayDirections.NegativeRight:
                direction = -Vector3.right;
                break;
        }

        if (Physics.Raycast(transform.position, direction, out hit, 999f, nodeMask))
        {
            var nodeHittedByRaycast = hit.collider.GetComponent<Node>();

            nodeHittedByRaycast._neighbors.Add(this);

            if (nodeHittedByRaycast.NeedToConnect == false)
            {
                Debug.Log($"The direction of {transform.name} is not correct," +
                $"the actual direction of this object is {direction}, incorrect node is painting on Red");

                GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        else
        {
            Debug.Log($"The direction of {transform.name} is not correct," +
                $"the actual direction of this object is {direction}");
        }
    }

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
