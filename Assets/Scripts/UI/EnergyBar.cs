using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private Utilities_ProgressBar progressBar;
    [SerializeField] private TextMeshProUGUI amountText;
    private void Awake()
    {
        EventManager.Instance?.Subscribe("OnEnergyUpdated", OnEnergyUpdated);
    }

    private void OnEnergyUpdated(params object[] parameters)
    {
        float current = (float) parameters[0];
        float max = (float) parameters[1];
        progressBar.SetMaxValue(max, false);
        progressBar.SetValue(current);

        amountText.text = $"{current}/{max}";
    }
}
