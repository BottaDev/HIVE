using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Utilities_Animation_GenericAnimationEvent : MonoBehaviour
{
    public UnityEvent animationEvent;

    public void TriggerEvent()
    {
        animationEvent.Invoke();
    }
}
