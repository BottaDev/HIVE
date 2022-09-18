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

    private NodeManager nodeManager;
    public float speed;
    public int currentWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        nodeManager = NodeManager.instance;

        nodeManager.CheckNodesNextToPlayer();

        currentWaypoint = 0;

        startingNode = nodeManager.ReturnClosestNodeFromPos(transform.position);

        goalNode = nodeManager.ReturnClosestNodeFromPos(player.transform.position);

        pathAStar = MakePath.ConstructPathAStar(startingNode, goalNode);

        if(pathAStar == null)
        {
            AuxMakePath();
        }

        if (pathAStar != null)
        {
            foreach (var item in pathAStar)
            {
                item.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
    }

    void AuxMakePath()
    {
        foreach (var item in nodeManager.allGoalNodes)
        {
            if(pathAStar == null)
            {
                pathAStar = MakePath.ConstructPathAStar(startingNode, item);
            }
        }
    }

    void Update()
    {
        if (pathAStar == null) return;

        //Enemy moves to player
        if(currentWaypoint < pathAStar.Count)
        {
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

        //Para FSM una vez hecho el primer camino hacia el player se abren 2 chances
        //1- Enemigo ve al jugador y le dispara
        //2- Enemigo NO ve al jugador y vuelve a checkear donde esta el player y va

    }
}
