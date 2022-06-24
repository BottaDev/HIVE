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
                    SceneManager.LoadScene("EmilrangBoss");
                    break;
                case 1:
                    winAmount++;
                    SceneManager.LoadScene("Win Screen");
                    break;

                default:
                    break;
            }
        }
    }
}
