using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRailInteractionPrompt : MonoBehaviour
{
    public GameObject prompt;
    [SerializeField] Rails rail;

    private void OnTriggerEnter(Collider other)
    {
        rail.WaitForInput(true);
        if (!rail.active)
        {
            prompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        rail.WaitForInput(false);
        prompt.SetActive(false);
    }
}
