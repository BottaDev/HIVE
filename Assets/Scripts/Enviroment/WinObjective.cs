using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinObjective : MonoBehaviour
{
    static int winAmount = 0;
    private void OnTriggerEnter(Collider other)
    {
        Player p = other.GetComponentInParent<Player>() ?? other.GetComponentInChildren<Player>();

        if (p != null)
        {
            p.SavePlayer();
            
            switch (winAmount)
            {
                case 0:
                    winAmount++;
                    SceneLoaderManager.instance.LoadScene(SceneLoaderManager.Scenes.EmilrangBoss);
                    break;
                case 1:
                    winAmount++;
                    SceneLoaderManager.instance.LoadScene(SceneLoaderManager.Scenes.WinScreen);
                    break;

                default:
                    break;
            }
        }
    }
}
