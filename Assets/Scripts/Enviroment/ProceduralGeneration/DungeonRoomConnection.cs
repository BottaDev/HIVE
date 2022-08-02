using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class DungeonRoomConnection : MonoBehaviour
{
    public ConnectionType type;
    DungeonRoom myRoom;
    public DungeonRoom connection;
    
    public enum ConnectionType
    {
        Horizontal, Vertical
    }

    public IEnumerator Connect()
    {
        myRoom = GetComponentInParent<DungeonRoom>();
        List<DungeonGenerator.DungeonRoomEntry>  possibleRoomsEntry = type == ConnectionType.Horizontal 
            ? DungeonGenerator.i.horizontalRooms
            : DungeonGenerator.i.verticalRooms;
        
        List<DungeonRoom>  possibleRooms = possibleRoomsEntry.Where(x => !x.special).Select(x => x.room).ToList();

        if (possibleRooms.Count > 0)
        {
            bool success = false;
            List<DungeonRoom> usedRooms = new List<DungeonRoom>();
        
            do
            {
                List<DungeonRoom> unusedRooms = possibleRooms.Where(x=>!usedRooms.Contains(x)).ToList();

                bool cutoffCondition = DungeonGenerator.i.size != -1 ? myRoom.DistanceFromStartingPoint >= DungeonGenerator.i.size : DungeonGenerator.i.shouldGenerateEnd;
                if (cutoffCondition && !DungeonGenerator.i.generatedEnd)
                {
                    //Place the end room

                    if (type == ConnectionType.Vertical && DungeonGenerator.i.useVerticalToHorizontalHelper)
                    {
                        DungeonRoom helper = Instantiate(DungeonGenerator.i.verticalToHorizontalHelper, myRoom.transform);
                    
                        PlaceAtConnection(helper);

                        DungeonGenerator.i.AddToDungeon(helper);
                        helper.DistanceFromStartingPoint = myRoom.DistanceFromStartingPoint + 1;
                        success = true;
                    }
                    else
                    {
                        DungeonRoom end = Instantiate(DungeonGenerator.i.endRoom, myRoom.transform);
                    
                        PlaceAtConnection(end);

                        end.generated = true;
                        success = true;
                        DungeonGenerator.i.AddToDungeon(end);
                        DungeonGenerator.i.generatedEnd = true;
                    }
                }
                else if (unusedRooms.Count == 0 || cutoffCondition)
                {
                    //No room can be placed or you've reached the limit
                    //Use a wall instead.
                    GameObject obj = Instantiate(DungeonGenerator.i.deadEnd, myRoom.transform);
                    
                    PlaceAtConnection(obj);

                    success = true;
                }
                else
                {

                    DungeonRoom randomRoom = null;
                    
                    if(possibleRoomsEntry.Any(x => x.index == myRoom.DistanceFromPlayer + 1))
                    {
                        DungeonGenerator.DungeonRoomEntry entry = possibleRoomsEntry
                            .Where(x => x.special && x.index == myRoom.DistanceFromPlayer + 1).ToList().ChooseRandom();
                        randomRoom = entry.room;
                    }
                    else
                    {
                        randomRoom = unusedRooms.ChooseRandom();
                    }

                    usedRooms.Add(randomRoom);
                    
                    DungeonRoom room = Instantiate(randomRoom, DungeonGenerator.i.transform);

                    PlaceAtConnection(room);

                    yield return new WaitForSeconds(DungeonGenerator.i.validationTime);
                
                    success = room.CheckBoundingBox();

                    if (!success)
                    {
                        Destroy(room.gameObject);
                        Debug.Log("Destroyed room");
                    }
                    else
                    {
                        if (connection != null)
                        {
                            Debug.Log("test");
                            DungeonGenerator.i.RemoveFromDungeon(connection);
                        }
                        
                        DungeonGenerator.i.AddToDungeon(room);
                        room.DistanceFromStartingPoint = myRoom.DistanceFromStartingPoint + 1;
                        room.DistanceFromPlayer = room.DistanceFromStartingPoint;
                        connection = room;
                    }
                }

                
            } while (!success);
        }
    }

    private void PlaceAtConnection(DungeonRoom room)
    {
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

        room.entrance.connection = myRoom;
    }
    
    private void PlaceAtConnection(GameObject obj)
    {
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
    }
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(DungeonRoomConnection))]
public class KamCustomEditor_DungeonRoomConnection : KamCustomEditor
{
    private DungeonRoomConnection editorTarget;

    private void OnEnable()
    {
        editorTarget = (DungeonRoomConnection)target;
    }

    public override void GameDesignerInspector()
    {
        EditorGUILayout.LabelField("Connections", EditorStyles.centeredGreyMiniLabel);

        editorTarget.type = (DungeonRoomConnection.ConnectionType) EditorGUILayout.EnumPopup(new GUIContent(
            "Connection Type",
            "This determines what kind of room can be connected in this connection."),
            editorTarget.type);
    }
}
#endif
#endregion
