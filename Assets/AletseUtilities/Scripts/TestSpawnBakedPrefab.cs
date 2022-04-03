using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnBakedPrefab : MonoBehaviour
{
    [SerializeField] private GameObject bakedPrefab;
    [SerializeField] private Vector3 position;
    
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(bakedPrefab, position, Quaternion.identity);
    }
}
