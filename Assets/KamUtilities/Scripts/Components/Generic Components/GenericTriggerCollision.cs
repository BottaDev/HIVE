using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTriggerCollision : MonoBehaviour
{
    public UnityEvent OnOnTriggerEnter;
    public UnityEvent OnOnTriggerStay;
    public UnityEvent OnOnTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        OnOnTriggerEnter?.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        OnOnTriggerStay?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        OnOnTriggerExit?.Invoke();
    }
}
