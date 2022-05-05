using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    Dictionary<string, Action<object[]>> _subscribers = new Dictionary<string, Action<object[]>>();

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

    public void Subscribe(string eventName, Action<object[]> callback)
    {
        string eventId = eventName.ToLower();
        if (!_subscribers.ContainsKey(eventId))
            _subscribers.Add(eventId, callback);
        else
            _subscribers[eventId] += callback;
    }

    public void Unsubscribe(string eventName, Action<object[]> callback)
    {
        string eventId = eventName.ToLower();
        if (!_subscribers.ContainsKey(eventId)) return;

        _subscribers[eventId] -= callback;
    }

    public void Trigger(string eventName, params object[] parameters)
    {
        string eventId = eventName.ToLower();
        if (!_subscribers.ContainsKey(eventId))
            return;

        _subscribers[eventId]?.Invoke(parameters);
    }
}
