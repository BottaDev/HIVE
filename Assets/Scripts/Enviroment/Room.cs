using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool[] connections = new bool[4]; //clockwise starting from north. 0 North - 1 East - 2 South - 3 West
    public RoomType type;
    public ShaderFloatLerper effect;
    public List<GameObject> deleteObjectsPostFade = new List<GameObject>();
    public Vector2Int mapPos { get; set; }
    public levelGen generator { get; set; }
    public bool ShouldFade { get; private set; }
    public bool Faded { get; private set; }
    public enum RoomType
    {
        X,T,L,I,Loop,Arena
    }

    public enum Connection
    {
        North = 0, East = 1, South = 2, West = 3
    }
    
    public int ConnectionsCount()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (connections[i]) count++;
        }

        return count;
    }
    
    public void FadeEmissive()
    {
        //Start your own effect.
        if (ShouldFade) return;
        
        ShouldFade = true;
        foreach (var obj in deleteObjectsPostFade)
        {
            if(obj != null)
            {
                try { Destroy(obj); } catch { }
            }
        }

        #region Start Effects of other rooms
            foreach (var room in GetNeighbors())
            {
                if (room != null)
                {
                    if (CheckRoomFade(room))
                    {
                        room.FadeEmissive();
                    }
                }
                
            }
        #endregion

    }

    private void OnTriggerEnter(Collider other)
    {
        if (AssetDatabase.i.playerMask.CheckLayer(other.gameObject.layer))
        {
            if (!Faded && ShouldFade)
            {
                Faded = true;
                effect.StartEffect();
            }
        }
    }

    public Room[] GetNeighbors()
    {
        bool[] adjustedConnections = AdjustConnectionsBasedOnRotation();
        
        Room[] neighbors = new Room[4];
        Room[,] map = generator.Map;
        Room[,] test = generator._map;

        
        Vector2Int nextPos = mapPos;
        if (adjustedConnections[(int) Connection.North])
        {
            //North
            nextPos = new Vector2Int(mapPos.x, mapPos.y+1);
            neighbors[(int) Connection.North] = map[nextPos.x, nextPos.y];
        }
            
        if (adjustedConnections[(int) Connection.East])
        {
            //East
            nextPos = new Vector2Int(mapPos.x+1, mapPos.y);
            neighbors[(int) Connection.East] = map[nextPos.x, nextPos.y];
        }
            
        if (adjustedConnections[(int) Connection.South])
        {
            //South
            nextPos = new Vector2Int(mapPos.x, mapPos.y-1);
            neighbors[(int) Connection.South] = map[nextPos.x, nextPos.y];
        }
            
        if (adjustedConnections[(int) Connection.West])
        {
            //West
            nextPos = new Vector2Int(mapPos.x-1, mapPos.y);
            neighbors[(int) Connection.West] = map[nextPos.x, nextPos.y];
        }

        return neighbors;
    }
    bool CheckRoomFade(Room room)
    {
        switch (room.type)
        {
            //All of these have more than one connection
            case RoomType.X:
            case RoomType.T:
                Room[] trueConnections = room.AdjustConnectionsBasedOnRotation().Zip(room.GetNeighbors(), (x, y) => Tuple.Create(x, y))
                    .Where(x=> x.Item1)
                    .Select(x => x.Item2)
                    .ToArray();

                return trueConnections.Count(x => x.ShouldFade) >= trueConnections.Length - 1;

            //All of these have only one connection, so its safe to turn them off.
            case RoomType.L:
            case RoomType.I:
            case RoomType.Loop:
            case RoomType.Arena:
                return true;
        }

        throw new Exception("Check room failed");
    }

    public bool[] AdjustConnectionsBasedOnRotation()
    {
        bool[] adjustedConnections = new bool[4];
        var rot = gameObject.transform.localRotation.eulerAngles.y;
        if (rot != 0)
        {
            switch (rot)
            {
                case 90:
                    //rotated to the right
                    adjustedConnections[(int) Connection.North] = connections[(int) Connection.West];
                    adjustedConnections[(int) Connection.East] = connections[(int) Connection.North];
                    adjustedConnections[(int) Connection.South] = connections[(int) Connection.East];
                    adjustedConnections[(int) Connection.West] = connections[(int) Connection.South];
                    break;
                
                case -180:
                case 180:
                    //rotated fully
                    adjustedConnections[(int) Connection.North] = connections[(int) Connection.South];
                    adjustedConnections[(int) Connection.East] = connections[(int) Connection.West];
                    adjustedConnections[(int) Connection.South] = connections[(int) Connection.North];
                    adjustedConnections[(int) Connection.West] = connections[(int) Connection.East];
                    break;
                
                case 270:
                case -90:
                    //rotated to the left
                    adjustedConnections[(int) Connection.North] = connections[(int) Connection.East];
                    adjustedConnections[(int) Connection.East] = connections[(int) Connection.South];
                    adjustedConnections[(int) Connection.South] = connections[(int) Connection.West];
                    adjustedConnections[(int) Connection.West] = connections[(int) Connection.North];
                    break;
            }
        }
        else
        {
            adjustedConnections = connections;
        }

        return adjustedConnections;
    }
}
