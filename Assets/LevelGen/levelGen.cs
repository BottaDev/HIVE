using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGen : MonoBehaviour
{
    [Header("Numbers")]
    public int spaceMod;
    public int minRoomQty;
    public int mapBorders;

    [Header("Objects")]
    public List<Room> LayoutRoomDB;
    public List<Room> RectRoomDB;
    public List<Room> LRoomDB;
    public List<Room> TRoomDB;
    public List<Room> CrossRoomDB;
    public List<Room> ArenaRoomDB;
    public Room startingRoom;
    public Room finalRoom;
    public GameObject mapContainer;

    private Room[,] Map = new Room[5, 5];
    private List<Room> arenaList = new List<Room>();
    private int changes;
    private int totalRooms = 0;
    
    public void StartGeneration()
    {
        Map = new Room[mapBorders, mapBorders];
        if (!mapContainer) mapContainer = this.gameObject;
        GenerateFullLevel();   
    }

    void GenerateFullLevel()
    {
        do
        {
            GenerateAdjacentRooms();
        } while (changes > 0);  //while (a < 6);

        if (totalRooms < minRoomQty) // || GetDeadEnds().Count < 4)
        {
            //print("bad map didn't laugh");
            DeleteMap();

            totalRooms = 0;
            GenerateFullLevel();
            return;
        }

        /*
        //Agrega bloques sólidos en los espacios vacíos del mapa. Esto es más bien inútil, pero es un poco cómodo para debuggear
        for (int i = 0; i < Map.GetLength(0); i++) //i es horizontal
        {
            for (int j = 0; j < Map.GetLength(1); j++) //j es vertical
            {
                if (!Map[i, j])
                {
                    Room e = Instantiate(roomNegativeOne, new Vector3(i, 0, j) * spaceMod + Vector3.down, Quaternion.identity);
                    e.transform.parent = mapContainer.transform;
                }
            }
        }
        */

        CleanMap();
        FillMap();
        ReplacecFarthestRoom(arenaList);
    }

    void GenerateAdjacentRooms()
    {
        totalRooms += changes;
        changes = 0;

        //Instancia la primer habitación si no está
        if (!Map[2, 0])
        {
            Map[2, 0] = startingRoom;

            //Room e = Instantiate(startingRoom, new Vector3(2, 0, 0) * spaceMod + Vector3.down, Quaternion.identity);
            //e.transform.parent = mapContainer.transform;
        }

        for (int i = 0; i < Map.GetLength(0); i++) //i es horizontal
        {
            for (int j = 0; j < Map.GetLength(1); j++) //j es vertical
            {
                if (Map[i, j])
                {

                    //Por cada habitación del mapa checkea por cada dirección cardinal:
                    //A. Que no apunte al exterior del mapa
                    //B. Que en su casilla adjacente no haya ya una habitación
                    //C. Que la habitación en cuestión tenga una puerta en aquella dirección
                    //Si se cumplen las 3, genera una habitación en ese lugar.

                    if (j < 4 && Map[i, j + 1] == null && Map[i, j].connections[0])
                    {
                        Map[i, j + 1] = SelectRandomRoom(2);

                        //Room e = Instantiate(SelectRandomRoom(2), new Vector3(i, 0, j + 1) * spaceMod + Vector3.down, Quaternion.identity);
                        //Map[i, j + 1] = e;
                        //e.transform.parent = mapContainer.transform;
                        changes++;
                    }

                    if (i < 4 && Map[i + 1, j] == null && Map[i, j].connections[1])
                    {
                        Map[i + 1, j] = SelectRandomRoom(3);

                        //Room e = Instantiate(SelectRandomRoom(3), new Vector3(i + 1, 0, j) * spaceMod + Vector3.down, Quaternion.identity);
                        //Map[i + 1, j] = e;
                        //e.transform.parent = mapContainer.transform;
                        changes++;
                    }

                    if (j > 0 && Map[i, j - 1] == null && Map[i, j].connections[2])
                    {
                        Map[i, j - 1] = SelectRandomRoom(0);

                        //Room e = Instantiate(SelectRandomRoom(0), new Vector3(i, 0, j - 1) * spaceMod + Vector3.down, Quaternion.identity);
                        //Map[i, j - 1] = e;
                        //e.transform.parent = mapContainer.transform;
                        changes++;
                    }

                    if (i > 0 && Map[i - 1, j] == null && Map[i, j].connections[3])
                    {
                        Map[i - 1, j] = SelectRandomRoom(1);

                        //Room e = Instantiate(SelectRandomRoom(1), new Vector3(i - 1, 0, j) * spaceMod + Vector3.down, Quaternion.identity);
                        //Map[i - 1, j] = e;
                        //e.transform.parent = mapContainer.transform;
                        changes++;
                    }
                }
            }
        }
        //Debug.Log("Changes = " + changes);
    }

    //Por cada sala:
    //A. Checkea hacia qué dirección tiene rooms a los que apunta pero que no le apuntan a él
    //B. Guarda las direcciones hacia las que tiene estas salidas incompatibles
    //C. Borra la habitación
    //D. Instancia un layout de habitación con las salidas compatibles
    void CleanMap()
    {
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                if (Map[i, j])
                {
                    int changes = 0;
                    bool[] cleanRoomConnections = new bool[4];
                    for (int b = 0; b < 4; b++)
                    {
                        cleanRoomConnections[b] = Map[i, j].connections[b];
                    }

                    for (int k = 0; k < 4; k++)
                    {
                        if (Map[i, j].connections[k] && !CheckRoomCoincidence(new Vector2Int(i, j))[k])
                        {
                            cleanRoomConnections[k] = false;
                            changes++;
                        }
                    }

                    if (changes > 0)
                    {
                        Map[i, j] = GetLayoutRoomPrefab(cleanRoomConnections);

                        //Destroy(Map[i, j].gameObject);
                        //Room e = Instantiate(GetLayoutRoomPrefab(cleanRoomConnections), new Vector3(i, 0, j) * spaceMod + Vector3.down, Quaternion.identity);
                        //e.transform.parent = mapContainer.transform;
                    }
                }
            }
        }
    }

    void FillMap()
    {
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                if (Map[i,j])
                {
                    Room current;

                    if(i == 2 && j == 0)
                        current = Instantiate(startingRoom, new Vector3(i, 0, j) * spaceMod + Vector3.down, Quaternion.identity);
                    else
                        current = Instantiate(GetRandomRoomPrefab(Map[i, j]), new Vector3(i, 0, j) * spaceMod + Vector3.down, Quaternion.identity);

                    current.transform.parent = mapContainer.transform;


                    switch (Map[i, j].ConnectionsCount())
                    {
                        case 1:
                                if (Map[i, j].connections[1])
                                {
                                    current.transform.eulerAngles += new Vector3(0, 90, 0);
                                    //Rotate it 90 degrees
                                }
                                if (Map[i, j].connections[2])
                                {
                                    current.transform.eulerAngles += new Vector3(0, 180, 0);
                                    //Rotate it 180 degrees
                                }
                                if (Map[i, j].connections[3])
                                {
                                    current.transform.eulerAngles += new Vector3(0, -90, 0);
                                    //Rotate it -90 degrees
                                }
                                arenaList.Add(current);

                            break;

                        case 2:

                            if ((Map[i, j].connections[0] && Map[i, j].connections[2]) || (Map[i, j].connections[1] && Map[i, j].connections[3]))
                            {
                                //Get a random Room from the Rect-Rooms Database

                                if (Map[i, j].connections[1])
                                {
                                    current.transform.eulerAngles += new Vector3(0, 90, 0);
                                    //Rotate it 90 degrees
                                }
                            }
                            else
                            {
                                //Get a random Room from the L-Rooms Database

                                if (Map[i, j].connections[0])
                                {
                                    if (Map[i, j].connections[3])
                                    {
                                        current.transform.eulerAngles += new Vector3(0, -90, 0);
                                        //rotate -90 degrees
                                    }
                                }
                                else if (Map[i, j].connections[2])
                                {
                                    if (Map[i, j].connections[3])
                                    {
                                        current.transform.eulerAngles += new Vector3(0, 180, 0);
                                        //rotate 180 degrees
                                    }
                                    else
                                    if (Map[i, j].connections[1])
                                    {
                                        current.transform.eulerAngles += new Vector3(0, 90, 0);
                                        //rotate 90 degrees
                                    }
                                }
                            }
                            break;

                        case 3:
                            //Get a random Room from the T-Rooms Database

                            if (!Map[i, j].connections[0])
                            {
                                current.transform.eulerAngles += new Vector3(0, 90, 0);
                                //rotate 90 degrees
                            }
                            else
                            if (!Map[i, j].connections[1])
                            {
                                current.transform.eulerAngles += new Vector3(0, 180, 0);
                                //rotate 180 degrees
                            }
                            else
                            if (!Map[i, j].connections[2])
                            {
                                current.transform.eulerAngles += new Vector3(0, -90, 0);
                                //rotate -90 degrees
                            }
                            break;

                        case 4:
                            //Get a random Room from the Cross Room Database
                            break;
                    }
                }
            }
        }
    }
    
    void ReplacecFarthestRoom(List<Room> arenaRooms)
    {
        Room farthest = arenaRooms[0];
        foreach (Room r in arenaRooms)
        {
            if (Vector3.Distance(Map[2, 0].transform.position, r.transform.position) > Vector3.Distance(Map[2, 0].transform.position, farthest.transform.position))
                farthest = r;
        }

        Room go = Instantiate(finalRoom, farthest.transform.position, farthest.transform.localRotation);
        go.transform.parent = mapContainer.transform;
        Destroy(farthest.gameObject);
    }

    Room GetRandomRoomPrefab(Room layoutRoom)
    {
        int randomInt;

        switch (layoutRoom.ConnectionsCount())
        {
            case 1:
                randomInt = Random.Range(0, ArenaRoomDB.Count);
                return (ArenaRoomDB[randomInt]);
                //Get a random Room from the Arena-Rooms Database

            case 2:

                if ((layoutRoom.connections[0] && layoutRoom.connections[2]) || (layoutRoom.connections[1] && layoutRoom.connections[3]))
                {
                    randomInt = Random.Range(0, RectRoomDB.Count);
                    return (RectRoomDB[randomInt]);
                    //Get a random Room from the Rect-Rooms Database
                }
                else
                {
                    randomInt = Random.Range(0, LRoomDB.Count);
                    return (LRoomDB[randomInt]);
                    //Get a random Room from the L-Rooms Database
                }

            case 3:
                randomInt = Random.Range(0, TRoomDB.Count);
                return (TRoomDB[randomInt]);
                //Get a random Room from the T-Rooms Database

            case 4:
                randomInt = Random.Range(0, CrossRoomDB.Count);
                return (CrossRoomDB[randomInt]);
                //Get a random Room from the Cross Room Database

            default:
                Debug.LogError("The requested room should have between 1 and 4 connections");
                break;
        }
        return null;
    }

    Room GetLayoutRoomPrefab(bool[] connections)
    {
        //should return the layoutRoom prefab with the requested connections

        foreach (Room r in LayoutRoomDB) //Checks every room layout
        {
            int equalExits = 0;
            for (int i = 0; i < 4; i++) //Checks every door
            {
                if (r.connections[i] == connections[i]) //If a door is the same as the requested it counts +1
                {
                    equalExits++;
                }
                //Debug.Log("Replaced: : 0-" + connections[0] + " 1-" + connections[1] + " 2-" + connections[2] + " 3-" + connections[3]);
                if (equalExits == 4) return r; //If a layout room has the same four doors as the requested then it is
            }
        }
        return null;
        //return roomNegativeOne;
    }

    public List<Vector2> GetDeadEnds()
    {
        List<Vector2> deadEndPositions = new List<Vector2>();

        for (int i = 0; i < Map.GetLength(0); i++) //i es horizontal
        {
            for (int j = 0; j < Map.GetLength(1); j++) //j es vertical
            {
                if (Map[i,j] && !(i == 2 && j == 0)) //checkea que la habitación esté y no sea la inicial
                {
                    if (CheckDeadEnd(new Vector2Int(i, j)))
                    {
                        deadEndPositions.Add(new Vector2(i, j));
                    }
                }
            }
        }

        return deadEndPositions;
    }


    //Recibe un número de puerta, devuelve una habitación con una puerta apuntando en esa dirección
    private Room SelectRandomRoom(int door) 
    {
        List<Room> filteredRoomDB = new List<Room>();   
        foreach (Room room in LayoutRoomDB)
        {
            if (room.connections[door] == true)
            {
                filteredRoomDB.Add(room);
            }
        }

        int rng;
        rng = Random.Range(0, filteredRoomDB.Count);
        return filteredRoomDB[rng];
    }

    private bool CheckDeadEnd(Vector2Int mapPos)
    {
        int connectedRooms = 0;

        foreach (bool a in CheckRoomCoincidence(mapPos))
        {
            if (a) connectedRooms++;
        }

        if (connectedRooms == 1)
        {
            return true;
        }

        return false;
    }

    //Recibe la posición de un room en el mapa y devuelve un array de bools que representan las direcciones hacia las que tiene otro room con el que se apuntan mutuamente
    bool[] CheckRoomCoincidence(Vector2Int mapPos)  
    {
        bool[] roomConnections = new bool[4];

        if (Map[mapPos.x, mapPos.y].connections[0])
        {
            if (mapPos.y != Map.GetLength(1) - 1 && Map[mapPos.x, mapPos.y + 1].connections[2])
            {
                roomConnections[0] = true;
            }
        }

        if (Map[mapPos.x, mapPos.y].connections[1])
        {
            if (mapPos.x != Map.GetLength(0) - 1 && Map[mapPos.x + 1, mapPos.y].connections[3])
            {
                roomConnections[1] = true;
            }
        }

        if (Map[mapPos.x, mapPos.y].connections[2])
        {
            if (mapPos.y != 0 && Map[mapPos.x, mapPos.y - 1].connections[0])
            {
                roomConnections[2] = true;
            }
        }

        if (Map[mapPos.x, mapPos.y].connections[3])
        {
            if (mapPos.x != 0 && Map[mapPos.x - 1, mapPos.y].connections[1])
            {
                roomConnections[3] = true;
            }
        }

        return roomConnections;
    }

    void DeleteMap()
    {
        foreach(Room r in mapContainer.GetComponentsInChildren<Room>())
        {
            Destroy(r.gameObject);
        }

        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                Map[i, j] = null;
            }
        }

        print("bad map deleted");
    }
}
