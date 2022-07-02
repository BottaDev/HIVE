using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinObjective : MonoBehaviour
{
    static int winAmount = 0;
    private bool used;
    private void OnTriggerEnter(Collider other)
    {
        if (!used)
        {
            Player p = other.GetComponentInParent<Player>() ?? other.GetComponentInChildren<Player>();

            if (p != null)
            {
                switch (winAmount)
                {
                    case 0:
                        winAmount++;
                        Emilrang.reloadedScene = false;
                        SceneLoaderManager.instance.LoadScene(SceneLoaderManager.Scenes.EmilrangBoss, SceneLoaderManager.SceneLoadType.normal);
                        break;
                    case 1:
                        winAmount++;
                        SceneLoaderManager.instance.LoadScene(SceneLoaderManager.Scenes.WinScreen);
                        break;

                    default:
                        break;
                }
                used = true;
            
                p.SavePlayer();
            }

            
        }
        
    }
}
