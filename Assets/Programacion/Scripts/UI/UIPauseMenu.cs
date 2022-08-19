using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenu : MonoBehaviour
{
    public static bool paused;
    public static UIPauseMenu i;

    public GameObject root;
    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        UnPause();
    }

    public void TogglePause()
    {
        if (paused)
        {
            UnPause();
        }
        else
        {
            Pause();
        }
    }
    public void Pause()
    {
        EventManager.Instance.Trigger("GamePause");

        paused = true;
        root.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnPause()
    {
        EventManager.Instance.Trigger("GameUnPause");
        
        paused = false;
        root.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
