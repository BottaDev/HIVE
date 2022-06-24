using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAndResumeIA : MonoBehaviour
{
    public void PauseAI()
    {
        var AI = FindObjectOfType<AI>();
        if (AI != null)
            AI.StopIA();
        else
            Debug.Log("No entro nadie al stop");
    }
    public void ContinueAI()
    {
        var AI = FindObjectOfType<AI>();
        if (AI != null)
            AI.ResumeIA();
        else
            Debug.Log("No entro nadie al continue");
    }
}
