using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UIMessageList : ObjectList
{
    private void Awake()
    {
        EventManager.Instance.Subscribe(EventManager.Events.OnSendUIMessage, SendMessage);
        EventManager.Instance.Subscribe(EventManager.Events.OnEliminateUIMessage, EliminateMessage);
        EventManager.Instance.Subscribe(EventManager.Events.OnSendUIMessageTemporary, SendMessageTemporary);
    }

    void SendMessage(params object[] p)
    {
        string message = (string)p[0];
        Color color = (Color)p[1];

        if (list.Where(x => x.GetComponent<TextMeshProUGUI>().text == message).ToArray().Length >= 1) return;
        

        GameObject obj = AddObject();
        UIMessage ui = obj.GetComponent<UIMessage>();
        ui.SetMessage(message);
        ui.SetColor(color);
    }

    void EliminateMessage(params object[] p)
    {
        string message = (string)p[0];

        GameObject obj = list.Find(x=> x.GetComponent<TextMeshProUGUI>().text == message);

        Remove(obj);
    }
    
    void SendMessageTemporary(params object[] p)
    {
        string message = (string)p[0];
        Color color = (Color)p[1];
        float time = (float) p[2];

        StartCoroutine(SendMessageTemporaryCoroutine(message, color, time));
    }

    IEnumerator SendMessageTemporaryCoroutine(string message, Color color, float time)
    {
        SendMessage(message, color);
        
        yield return new WaitForSeconds(time);

        EliminateMessage(message);
    }
}
