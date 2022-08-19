using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomValidator : MonoBehaviour
{
    public DungeonRoomConnection connection;

    private void OnTriggerEnter(Collider other)
    {
        if (DungeonGenerator.i.boundingBoxMask.CheckLayer(other.gameObject.layer))
        {
            
        }
    }
}
