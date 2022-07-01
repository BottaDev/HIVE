using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseScreen : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Win()
    {
        SceneLoaderManager.instance.LoadScene(SceneLoaderManager.Scenes.MainMenu);
    }
    public void Restart()
    {
        SceneLoaderManager.instance.LoadLastScene();
    }
}
