using System;
using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionsMenu : MonoBehaviour
{
    public Slider sensitivitySlider;
    public float min = 0.5f;
    public float max = 6;
    private void Start()
    {
        if(PlayerPrefs.HasKey("Hive_MouseSensitivity"))
        {
            SettingsManager.mouseSensitivity = PlayerPrefs.GetFloat("Hive_MouseSensitivity");
            float value = KamUtilities.Map(SettingsManager.mouseSensitivity, min, max, 0,1);
            sensitivitySlider.value = value;
        }
        else
        {
            sensitivitySlider.value = 0.5f;
        }
    }

    public void SetSensitivity(Slider value)
    {
        float sensitivity = KamUtilities.Map(value.value, 0, 1, min, max);
        SettingsManager.mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("Hive_MouseSensitivity", sensitivity);
    }
}
