using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public enum Events
    {
        OnNavMeshBake,
        OnEnemyDamaged,
        OnPlayerDamaged,
        OnEnemyCounted,
        OnTotalEnemy,
        OnEnemyDeath,
        OnPlayerDead,
        OnLifeUpdated,
        OnPlayerRailAttached,
        OnPlayerRailDeAttached,
        OnPlayerRailActive,
        OnPlayerDashCd,
        OnPlayerGrappleCd,
        OnPlayerGrenadeCd,
        OnPlayerDashUnlock,
        OnPlayerGrappleUnlock,
        OnPlayerGrenadeUnlock,
        OnPlayerUpdateAmmo,
        NeedsPlayerReference,
        OnPlayerLevelSystemUpdate,
        OnEnergyUpdated,
        OnSendUIMessage,
        OnEliminateUIMessage,
        OnPlayerDirectHookshotUnlock,
        OnPlayerDirectHookshotCD,
        OnSendUIMessageTemporary,
        OnPlayerEnteredUpgradeRoom,
        OnPlayerLeftUpgradeRoom
    }

    public static EventManager Instance { get; private set; }

    Dictionary<Events, Action<object[]>> _subscribers = new Dictionary<Events, Action<object[]>>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("Duplicated detected found" + gameObject.name);
            Destroy(this);
        }
    }

    public void Subscribe(Events eventId, Action<object[]> callback)
    {
        if (!_subscribers.ContainsKey(eventId))
            _subscribers.Add(eventId, callback);
        else
            _subscribers[eventId] += callback;
    }

    public void Unsubscribe(Events eventId, Action<object[]> callback)
    {
        if (!_subscribers.ContainsKey(eventId)) return;

        _subscribers[eventId] -= callback;
    }

    public void Trigger(Events eventId, params object[] parameters)
    {
        if (!_subscribers.ContainsKey(eventId))
            return;

        _subscribers[eventId]?.Invoke(parameters);
    }
}
