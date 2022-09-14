using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPathAStar : MonoBehaviour
{
    public Transform player;
    public List<Node> pathAStar;
    private Node startingNode;
    private Node goalNode;
    //Vector3 velocity;

    public float speed;
    public int currentWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentWaypoint = 0;

        startingNode = AllNodes.instance.ReturnClosestNodeFromPos(transform.position);

        goalNode = AllNodes.instance.ReturnClosestNodeFromPos(AllNodes.instance.playerTransform.position);

        pathAStar.Clear();

        pathAStar = MakePath.ConstructPathAStar(startingNode, goalNode);

    }

    // Update is called once per frame
    void Update()
    {
        var comparatedNode = AllNodes.instance.ReturnClosestNodeFromPos(AllNodes.instance.playerTransform.position);

        if (pathAStar.Count == 0)
        {
            currentWaypoint = 0;

            pathAStar = MakePath.ConstructPathAStar(AllNodes.instance.ReturnClosestNodeFromPos(transform.position),
                AllNodes.instance.ReturnClosestNodeFromPos(AllNodes.instance.playerTransform.position));
        }

        if (goalNode != comparatedNode && pathAStar != null)
        {
            //pathAStar.Clear();
            //currentWaypoint = 0;
            //startingNode = AllNodes.instance.ReturnClosestNodeFromPos(transform.position);
            //goalNode = comparatedNode;
            //pathAStar = MakePath.ConstructPathAStar(startingNode, goalNode);


            if (pathAStar.Last() == null) return;

            var lastNodeFromPath = pathAStar.Last();

            var nextNodeFromOldPath = AllNodes.instance.ReturnClosestNodeFromPos(lastNodeFromPath.transform.position);

            var newStartNode = AllNodes.instance.ReturnClosestNodeFromPos(nextNodeFromOldPath.transform.position);

            goalNode = AllNodes.instance.ReturnClosestNodeFromPos(AllNodes.instance.playerTransform.position);

            var newPath = MakePath.ConstructPathAStar(newStartNode, goalNode);

            foreach (var item in newPath)
            {
                pathAStar.Add(item);
            }
        }

       
        if (pathAStar != null)
        {
            foreach (var item in pathAStar)
            {
                item.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }

        //if (pathAStar.Count == 0) return;

        Vector3 dir = pathAStar[currentWaypoint].transform.position - transform.position;
        transform.position += transform.forward * speed * Time.deltaTime;
        transform.forward = dir;

        if (dir.magnitude < 0.3f)
        {
            currentWaypoint++;

            if (currentWaypoint > pathAStar.Count - 1)
            {
                foreach (var item in pathAStar)
                {
                    item.GetComponent<MeshRenderer>().material.color = Color.gray;
                }
                pathAStar.Clear();
            }
        }
    }

    private void OnDrawGizmos()
    {
    }

    private void MoveToPath(Vector3 target)
    {
    
    }

    private void GoToWaypoint()
    {
      
    }
}
