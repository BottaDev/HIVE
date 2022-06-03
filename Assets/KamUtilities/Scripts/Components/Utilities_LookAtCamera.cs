using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities_LookAtCamera : MonoBehaviour
{
    [Tooltip("This transform will always look at the camera")]
    public Transform transform;
    public Vector3 rotationOffset;

    [Header("Camera")]
    public Camera cameraToUse;
    public bool useMainCamera;

    private void Start()
    {
        if(useMainCamera)
        {
            cameraToUse = Camera.main;
        }
    }

    private void LateUpdate()
    {
        transform?.LookAt(cameraToUse.transform);
        var rot = transform.rotation.eulerAngles;
        transform.Rotate(rotationOffset);
        rot += rotationOffset;
        
        transform.SetPositionAndRotation(transform.position, Quaternion.Euler(rot));
    }
}
