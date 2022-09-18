using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager instance;

    public Transform playerTransform;

    //Change list to HarshSet for better performance, Error allNodes[i] Not Found
    public List<Node> allNodes = new List<Node>();

    public List<Node> allGoalNodes = new List<Node>();

    public GameObject checker;

    private void Awake()
    {
        instance = this;

        playerTransform = FindObjectOfType<Player>().transform;

        var findAllNodes = Resources.FindObjectsOfTypeAll<Node>();

        for (int i = 0; i < findAllNodes.Length; i++)
        {
            allNodes.Add(findAllNodes[i]);
        }


    }

    public Node ReturnClosestNodeFromPos(Vector3 pos)
    {
        int returnIntValue = 0;
        float closestDistance = 10f;

        for (int i = 0; i < allNodes.Count; i++)
        {
            float distanceToNode = (allNodes[i].gameObject.transform.position - pos).magnitude;

            if (distanceToNode < closestDistance)
            {
                closestDistance = distanceToNode;
                returnIntValue = i;
            }
        }
        return allNodes[returnIntValue];
    }

    public void CheckNodesNextToPlayer()
    {
        allGoalNodes.Clear();
        Instantiate(checker, playerTransform.position, Quaternion.identity);
    }
}
