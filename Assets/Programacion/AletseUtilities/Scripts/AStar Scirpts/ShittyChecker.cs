using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShittyChecker : MonoBehaviour
{
    public float radius;
    public LayerMask layer;
    public Vector3 newPos;

    private void Awake()
    {
        //StartCoroutine("waitBeforeCheckGoalNodes");
        ReturnGoalNodes();
    }

    IEnumerator waitBeforeCheckGoalNodes()
    {
        var wait = new WaitForSeconds(2f);
        return null;
    }

    public void ReturnGoalNodes()
    {
        List<Node> goalNodes = new List<Node>();

        var overlapNodes = Physics.OverlapSphere(NodeManager.instance.playerTransform.position + newPos, radius, layer);

        for (int i = 0; i < overlapNodes.Length; i++)
        {
            goalNodes.Add(overlapNodes[i].GetComponent<Node>());
        }

        NodeManager.instance.allGoalNodes = goalNodes;

        //StopAllCoroutines();

        Destroy(gameObject);
    }

    //private void Update()
    //{
    //    var overlapNodes = Physics.OverlapSphere(transform.position + newPos, radius, layer);
    //
    //    for (int i = 0; i < overlapNodes.Length; i++)
    //    {
    //        nodes.Add(overlapNodes[i].GetComponent<Node>());
    //    }
    //}
    //
    //private void OnTriggerEnter(Collider other)
    //{
    //    var overlapNodes = Physics.OverlapSphere(transform.position + newPos, radius, layer);
    //
    //    for (int i = 0; i < overlapNodes.Length; i++)
    //    {
    //        nodes.Add(overlapNodes[i].GetComponent<Node>());
    //    }
    //}
    //
    //private void OnTriggerExit(Collider other)
    //{
    //    var overlapNodes = Physics.OverlapSphere(transform.position + newPos, radius, layer);
    //
    //    for (int i = 0; i < overlapNodes.Length; i++)
    //    {
    //        nodes.Remove(overlapNodes[i].GetComponent<Node>());
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + newPos, radius);
    }
}
