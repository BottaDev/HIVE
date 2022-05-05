using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPosition : MonoBehaviour
{
    public GameObject enemyPrefab;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe("OnNavMeshBake", OnNavMeshBake);
    }

    void OnNavMeshBake(params object[] parameters)
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
