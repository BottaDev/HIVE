using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour
{
    public TextMeshProUGUI text;


    // Update is called once per frame
    void Update()
    {
        var intTime = (int)Time.time;
        var minutes = intTime / 60;
        var seconds= intTime % 60;
        var fraction= Time.time * 1000;
        fraction = fraction % 1000;
        text.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
    }
}
