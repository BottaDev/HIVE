using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UIMechanicUnlock : MonoBehaviour
{
    [SerializeField] private EventManager.Events eventType;
    [SerializeField] private UnityEvent onTrigger;
    
    private void Awake()
    {
        EventManager.Instance.Subscribe(eventType, Trigger);
    }

    private void Trigger(params object[] p)
    {
        onTrigger?.Invoke();
    }
}
