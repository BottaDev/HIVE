using System;
using System.Collections;
using System.Collections.Generic;
using IA2;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EmilrangBodyPart : MonoBehaviour
{
    public EmilrangBodyOpening[] openings;
    public Utilities_ConstantlyRotate rotate;
    public Utilities_SinewaveMovement sine;
    
    public void Unpause()
    {
        foreach (var opening in openings)
        {
            opening.Unpause();
        }
        rotate.Resume();
        sine.Resume();
    }
    
    public void Pause()
    {
        foreach (var opening in openings)
        {
            opening.Pause();
        }
        rotate.Stop();
        sine.Stop();
    }

    public void SendInput(EmilrangBodyOpening.Attack attack, int index = -1)
    {
        if (index != -1)
        {
            openings[index].SendInput(attack);
        }
        else
        {
            foreach (var opening in openings)
            {
                opening.SendInput(attack);
            }
        }
        
    }

    public void StartPhase2()
    {
        foreach (var opening in openings)
        {
            opening.SendInput(opening.phase2Attack);
        }
    }

    public void StartPhase3()
    {
        foreach (var opening in openings)
        {
            opening.SendInput(opening.phase3Attack);
        }
    }
}
