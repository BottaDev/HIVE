using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    public DungeonRoomConnection entrance;
    public DungeonRoomConnection[] exits;
    public BoxCollider boundingBox;
    public Utilities_TriggerColliderList collisionList;
    
    public bool generated = false;

    public int DistanceFromStartingPoint;
    
    public IEnumerator Generate()
    {
        if (!generated)
        {
            foreach (var exit in exits)
            {
                yield return exit.Connect();
                //Exit was connected.
            }

            generated = true;
        }
    }

    public bool CheckBoundingBox()
    {
        return collisionList.colliders.Count == 0;
        //Physics.OverlapBox(boundingBox.center, boundingBox.size * 0.5f, boundingBox.transform.rotation, DungeonGenerator.i.boundingBoxMask).Length == 1;
    }
}
