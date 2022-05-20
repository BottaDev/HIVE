using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{
    public List<Utilities_ImageProgressBar> ammoBars;

    private void Awake()
    {
        EventManager.Instance.Subscribe("OnPlayerUpdateAmmo", UpdateBar);
    }

    public void UpdateBar(params object[] p)
    {
        int currentAmmo = (int)p[0];
        int maxAmmount = (int)p[1];

        foreach (var ammoBar in ammoBars)
        {
            ammoBar.SetValue(currentAmmo);
            ammoBar.SetMaxValue(maxAmmount);
        }
    }
}
