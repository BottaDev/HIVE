using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UIMessageList : ObjectList
{
    private Dictionary<string, Coroutine> coroutines = new Dictionary<string, Coroutine>();
    private void Awake()
    {
        EventManager.Instance.Subscribe("OnSendUIMessage", SendMessage);
        EventManager.Instance.Subscribe("OnEliminateUIMessage", EliminateMessage);
        EventManager.Instance.Subscribe("OnSendUIMessageTemporary", SendMessageTemporary);
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

        SendMessage(message, color);
        
        if (!coroutines.ContainsKey(message))
        {
            coroutines.Add(message, StartCoroutine(SendMessageTemporaryCoroutine(message, color, time)));
        }
        else
        {
            StopCoroutine(coroutines[message]);
            coroutines[message] = StartCoroutine(SendMessageTemporaryCoroutine(message, color, time));
        }
    }

    IEnumerator SendMessageTemporaryCoroutine(string message, Color color, float time)
    {
        yield return new WaitForSeconds(time);

        EliminateMessage(message);
    }
}
