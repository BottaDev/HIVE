using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetMessage(string text)
    {
        this.text.text = text;
    }

    public void SetColor(Color color)
    {
        text.color = color;
    }
}
