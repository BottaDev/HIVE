using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool[] connections = new bool[4]; //clockwise starting from north. 0 North - 1 East - 2 South - 3 West

    public int ConnectionsCount()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (connections[i]) count++;
        }

        return count;
    }
}
