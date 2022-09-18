using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShittySpawner : MonoBehaviour
{
    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(enemy, transform.position, Quaternion.identity);
    }

    
}
