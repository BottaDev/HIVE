using System.Collections.Generic;
using UnityEngine;

public class AllNodes : MonoBehaviour          
{
    public static AllNodes instance;
    public List<Node> allNodes = new List<Node>();

    private void Awake()
    {
        instance = this;
    }

    public Node ReturnClosestNodeFromPos(Vector3 pos)
    {
        int returnIntValue = 0;
        float closestDistance = 10f;
        for (int i = 0; i < allNodes.Count; i++)
        {
            float distanceToNode = (allNodes[i].gameObject.transform.position - pos).magnitude;
            if(distanceToNode < closestDistance)
            {
                closestDistance = distanceToNode;
                returnIntValue = i;
            }
        }
        return allNodes[returnIntValue];
    }   
}
