using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChanceToSpawn : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnItem
    {
        public string name;
        [Range(0,100)]
        public int chanceToSpawn;
        public List<GameObject> objects;
    }

    public List<SpawnItem> spawns;

    public void Start()
    {
        foreach (var spawn in spawns)
        {
            if (Random.Range(0, 101) > spawn.chanceToSpawn)
            {
                foreach (GameObject obj in spawn.objects)
                {
                    Destroy(obj);
                }
            }
        }
    }
}
