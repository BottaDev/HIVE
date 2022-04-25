using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.PlayMusic(AssetDatabase.i.GetSong(Songs.Level1));
    }
}