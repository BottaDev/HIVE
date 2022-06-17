using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator i;
    public LayerMask boundingBoxMask;
    public DungeonRoom startRoom;
    public Vector3 startingPoint;
    public GameObject deadEnd;
    public List<DungeonRoom> horizontalRooms;
    public List<DungeonRoom> verticalRooms;
    public float validationTime;
    public Action generation = delegate {  };

    private List<DungeonRoom> dungeon = new List<DungeonRoom>();
    private List<DungeonRoom> ungeneratedRooms = new List<DungeonRoom>();
    private void Awake()
    {
        i = this;
    }

    public void Start()
    {
        StartCoroutine(GenerateDungeon());
    }

    private IEnumerator GenerateDungeon()
    {
        AddToDungeon(Instantiate(startRoom, startingPoint, Quaternion.identity));

        while (ungeneratedRooms.Count > 0)
        {
            DungeonRoom room = ungeneratedRooms[0];
            
            yield return room.Generate();
            
            Debug.Log("Room generated.");
            ungeneratedRooms = dungeon.Where(x => !x.generated).ToList();
        }
        
        Debug.Log("Dungeon generated.");
    }

    public void AddToDungeon(DungeonRoom room)
    {
        dungeon.Add(room);
        ungeneratedRooms = dungeon.Where(x => !x.generated).ToList();
    }

    public List<DungeonRoom> GetRoomsOfType(DungeonRoomConnection.ConnectionType type)
    {
        List<DungeonRoom> result = new List<DungeonRoom>();
        switch (type)
        {
            case DungeonRoomConnection.ConnectionType.Horizontal:
                result.AddRange(horizontalRooms);
                break;
            
            case DungeonRoomConnection.ConnectionType.Vertical:
                result.AddRange(verticalRooms);
                break;
        }
        return result;
    }
}
