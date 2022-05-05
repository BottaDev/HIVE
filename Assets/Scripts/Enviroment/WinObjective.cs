using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinObjective : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player p = other.GetComponentInParent<Player>() ?? other.GetComponentInChildren<Player>();

        if (p != null)
        {
            p.SavePlayer();
            SceneManager.LoadScene("Win Screen");
        }
    }
}
