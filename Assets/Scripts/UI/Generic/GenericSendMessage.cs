using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSendMessage : MonoBehaviour
{
    public string message;
    public Color color;
    public float timeOnScreen;

    public void Send()
    {
        EventManager.Instance.Trigger("OnSendUIMessage", message, color);
    }

    public void Delete()
    {
        EventManager.Instance.Trigger("OnEliminateUIMessage", message);
    }

    public void SendTemporary()
    {
        EventManager.Instance.Trigger("OnSendUIMessageTemporary", message, color, timeOnScreen);
    }
}
