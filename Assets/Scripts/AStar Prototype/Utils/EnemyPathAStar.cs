using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathAStar : MonoBehaviour
{
    public GameObject player;
    private List<Node> path;
    public Node startingNode;
    public Node goalNode;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        startingNode = AllNodes.instance.ReturnClosestNodeFromPos(transform.position);

        goalNode = AllNodes.instance.ReturnClosestNodeFromPos(player.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            path = MakePath.instance.ConstructPathAStar(startingNode, goalNode);
        }
       
    }

    private void OnDrawGizmos()
    {
    }

    private void MoveToPath(Vector3 target)
    {
        velocity = target - transform.position;
        velocity.Normalize();
        velocity *= 2f;
    }

    private void GoToWaypoint()
    {
      
    }
}
