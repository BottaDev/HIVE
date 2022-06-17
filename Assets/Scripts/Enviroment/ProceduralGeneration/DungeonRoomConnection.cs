using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonRoomConnection : MonoBehaviour
{
    public ConnectionType type;
    DungeonRoom myRoom;
    public DungeonRoom connection;
    
    public enum ConnectionType
    {
        Horizontal, Vertical
    }

    private void Start()
    {
        myRoom = GetComponentInParent<DungeonRoom>();
    }

    public IEnumerator Connect()
    {
        List<DungeonRoom>  possibleRooms = type == ConnectionType.Horizontal 
            ? DungeonGenerator.i.horizontalRooms
            : DungeonGenerator.i.verticalRooms;

        if (possibleRooms.Count > 0)
        {
            bool success = false;
            List<DungeonRoom> usedRooms = new List<DungeonRoom>();
        
            do
            {
                List<DungeonRoom> unusedRooms = possibleRooms.Where(x=>!usedRooms.Contains(x)).ToList();

                
                if (unusedRooms.Count == 0)
                {
                    //You've tried every room, and none can be placed. Use a wall instead.
                    GameObject obj = Instantiate(DungeonGenerator.i.deadEnd, myRoom.transform);
                    
                    #region Place the wall
                    Transform originalParent = obj.transform.parent;

                    //Set parent to its entrance connection
                    obj.transform.parent = transform;

                    //Set position to this connection, and rotate to match
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.forward = -transform.forward;
            
                    //Revert back your changes

                    obj.transform.parent = originalParent;
                    #endregion

                    success = true;
                }
                else
                {
                    DungeonRoom randomRoom = unusedRooms.ChooseRandom();
                    usedRooms.Add(randomRoom);
                    
                    DungeonRoom room = Instantiate(randomRoom, DungeonGenerator.i.transform);

                    #region Place the room
                    Transform originalParent = room.transform.parent;
                    Transform entranceOriginalParent = room.entrance.transform.parent;

                    //Set room parent to its entrance connection
                    room.entrance.transform.parent = transform;
                    room.transform.parent = room.entrance.transform;

                    //Set entrance's position to this connection, and rotate to match
                    room.entrance.transform.localPosition = Vector3.zero;
                    room.entrance.transform.forward = -transform.forward;
            
                    //Revert back your changes

                    room.transform.parent = originalParent;
                    room.entrance.transform.parent = entranceOriginalParent;
                    #endregion

                    yield return new WaitForSeconds(DungeonGenerator.i.validationTime);
                
                    success = room.CheckBoundingBox();

                    if (!success)
                    {
                        Destroy(room.gameObject);
                        Debug.Log("Destroyed room");
                    }
                    else
                    {
                        DungeonGenerator.i.AddToDungeon(room);
                        connection = room;
                    }
                }

                
            } while (!success);
        }
    }
}
