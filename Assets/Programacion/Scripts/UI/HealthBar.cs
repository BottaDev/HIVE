using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Utilities_ImageLazyProgressBar progressBar;
    [SerializeField] private TextMeshProUGUI text;
    private void Awake()
    {
        EventManager.Instance?.Subscribe("OnLifeUpdated", OnLifeUpdated);
    }

    private void OnLifeUpdated(params object[] parameters)
    {
        int currentHp = (int) parameters[0];
        int maxHp = (int) parameters[1];
        progressBar.SetMaxValue(maxHp, false);
        progressBar.SetValue(currentHp);

        text.text = $"{currentHp}/{maxHp}";
    }
}
