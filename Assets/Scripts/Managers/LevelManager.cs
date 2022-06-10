using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    [Header("Objective")] 
    [Tooltip("The minimum number of enemies to kill to open the exit")] public int minEnemies = 10;
    
    private GameObject _exitDoor;
    private int _enemiesKilled = 0;
    private levelGen _levelGen;

    private void Awake()
    {
        _levelGen = GetComponent<levelGen>();
        
        EventManager.Instance.Subscribe("OnEnemyDeath", OnEnemyDeath);
    }

    private void Start()
    {
        // Generate the level
        _levelGen.StartGeneration();
        _exitDoor = GameObject.FindWithTag("Door");
        
        // Bake map navmesh
        //FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        
        EventManager.Instance.Trigger("OnTotalEnemy", minEnemies);
    }
    
    private void OnEnemyDeath(params object[] parameters)
    {
        _enemiesKilled++;
        GameStats.enemiesKilled++;
        UIExtraInfoScreen.i.UpdateStats();
        EventManager.Instance.Trigger("OnEnemyCounted", _enemiesKilled);

        // Open the exit door
        if (_enemiesKilled >= minEnemies && _exitDoor.activeSelf)
            _exitDoor.SetActive(false);
    }
}
