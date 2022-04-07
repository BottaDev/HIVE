using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            SceneManager.LoadScene(1);
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(EventManager.Events.OnPlayerDead, OnPlayerDead);
    }

    void OnPlayerDead(params object[] parameters)
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        SceneManager.LoadScene(2);
    }
}
