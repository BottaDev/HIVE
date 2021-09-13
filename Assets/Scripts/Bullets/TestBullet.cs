using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public float speed = 10f;
    
    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);    
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
