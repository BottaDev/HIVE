using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionGetter : MonoBehaviour
{
    Material tubeMaterial;
    public Transform playerTransform;
    public Vector3 playerPos;
    private void Awake()
    {
        tubeMaterial = GetComponent<Renderer>().material;
        playerTransform = FindObjectOfType<Player>().transform;
    }


    void Update()
    {
        playerPos = playerTransform.position;
        tubeMaterial.SetVector("_PlayerPosition", playerPos);
    }
}
