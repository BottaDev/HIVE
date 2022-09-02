using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneForcer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwapScene(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwapScene(5);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SwapScene(6);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SwapScene(7);
    }

    void SwapScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
