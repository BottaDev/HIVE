using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{
    public string winScreen;
    public string loseScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            SceneManager.LoadScene(winScreen);
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe("OnLifeUpdated", Lose);
    }

    void Lose(params object[] parameters)
    {
        if ((float) parameters[0] <= 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            SceneManager.LoadScene(loseScreen);
        }
    }
}
