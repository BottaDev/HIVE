using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generic_PlayMusic : MonoBehaviour
{
    public Songs song;
    private void Start()
    {
        AudioManager.instance.PlayMusic(AssetDatabase.i.GetSong(song));
    }
}