using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTriggerEnterAndExitEvent : MonoBehaviour
{
    public LayerMask triggerable;
    public string entered;
    public string left;
    
    private void OnTriggerEnter(Collider other)
    {
        if (triggerable.CheckLayer(other.gameObject.layer))
        {
            EventManager.Instance.Trigger(entered);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (triggerable.CheckLayer(other.gameObject.layer))
        {
            EventManager.Instance.Trigger(left);
        }
    }
}
