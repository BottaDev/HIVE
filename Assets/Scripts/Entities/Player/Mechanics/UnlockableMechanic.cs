using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableMechanic : MonoBehaviour
{
    [Header("Mechanic")]
    protected bool mechanicUnlocked;
    public EventManager.Events unlockEvent;
    [SerializeField] private bool unlockedAtTheStart;

    public void Start()
    {
        if (unlockedAtTheStart)
        {
            Unlock();
        }
    }

    public void Unlock()
    {
        mechanicUnlocked = true;
        EventManager.Instance.Trigger(unlockEvent);
    }

    public void Lock()
    {
        mechanicUnlocked = false;
    }
}
