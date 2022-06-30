using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    public DungeonRoomConnection entrance;
    public DungeonRoomConnection[] exits;
    public BoxCollider boundingBox;
    public Utilities_TriggerColliderList collisionList;
    
    public bool generated = false;
    public bool visited = false;
    public int DistanceFromStartingPoint;
    public int DistanceFromPlayer;
    
    public IEnumerator Generate()
    {
        if (!generated)
        {
            if (DungeonGenerator.i.size == -1)
            {
                while (DistanceFromPlayer >= DungeonGenerator.i.roomAmountBeforeAndAfterPlayer && !DungeonGenerator.i.shouldGenerateEnd)
                {
                    yield return null;
                }
            }
            
            foreach (var exit in exits)
            {
                yield return exit.Connect();
                //Exit was connected.
            }

            generated = true;
        }
    }

    public void Visit()
    {
        if(DungeonGenerator.i.generatedEnd) return;
        
        Debug.Log("Visit");
        visited = true;
        DistanceFromPlayer = 0;
        
        StartCoroutine(UpdateDistanceFromPlayer(this));
    }

    public IEnumerator UpdateDistanceFromPlayer(DungeonRoom lastRoom)
    {
        if (generated)
        {
            //IA2-P1
            //Esto reemplaza a "Concat", porque literalmente append es concat para un elemento solo.
            List<DungeonRoomConnection> connections = entrance != null ? exits.Append(entrance).ToList() : exits.ToList();
            
            foreach (var exit in connections)
            {
                if (exit.connection != lastRoom)
                {
                    exit.connection.DistanceFromPlayer = DistanceFromPlayer + 1;
                    
                    if (exit.connection.DistanceFromPlayer > DungeonGenerator.i.roomAmountBeforeAndAfterPlayer)
                    {
                        DungeonGenerator.i.RemoveFromDungeon(exit.connection);
                    }
                    
                    yield return DungeonGenerator.i.validationTime + 0.001f;
                    yield return exit.connection.UpdateDistanceFromPlayer(this);
                }
            }
        }
    }

    public bool CheckBoundingBox()
    {
        return collisionList.colliders.Count == 0;
        //Physics.OverlapBox(boundingBox.center, boundingBox.size * 0.5f, boundingBox.transform.rotation, DungeonGenerator.i.boundingBoxMask).Length == 1;
    }
}
