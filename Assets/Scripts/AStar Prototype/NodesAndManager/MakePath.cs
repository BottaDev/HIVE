using System.Collections.Generic;
using UnityEngine;

public class MakePath : MonoBehaviour
{
    public static MakePath instance;

    public List<Node> pathPlayer;

    void Awake()
    {
        instance = this;
    }

    public List<Node> ConstructPathAStar(Node startingNode, Node goalNode)
    {
        if (pathPlayer != null) pathPlayer.Clear();
        else pathPlayer = new List<Node>();

        PriorityQueue frontier = new PriorityQueue();
        frontier.Put(startingNode, 0);
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        cameFrom.Add(startingNode, null);
        costSoFar.Add(startingNode, 0);

        while (frontier.Count() > 0)
        {
            Node current = frontier.Get();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();
                Node nodeToAdd = current;
                while (nodeToAdd != null)
                {
                    pathPlayer.Add(nodeToAdd);
                    path.Add(nodeToAdd);
                    nodeToAdd = cameFrom[nodeToAdd];
                }
                path.Reverse();
                return path;
            }

            foreach (Node next in current.GetNeighbors())
            {
                int newCost = costSoFar[current] + next.cost;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    if (costSoFar.ContainsKey(next))
                    {
                        costSoFar[next] = newCost;
                        cameFrom[next] = current;
                    }
                    else
                    {
                        cameFrom.Add(next, current);
                        costSoFar.Add(next, newCost);
                    }
                    float priority = newCost + Vector3.Distance(next.transform.position, goalNode.transform.position);
                    frontier.Put(next, priority);

                }
            }
        }
        return default;
    }
}
