using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoppingCollider : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("algo entro");
        var emilrang = other.GetComponent<Emilrang>();

        if(emilrang != null)
        {
            Debug.Log("entro emilrang stop");
            emilrang.speed = 0f;
        }
        else
        {
            Debug.Log("xd");
        }
    }
}
