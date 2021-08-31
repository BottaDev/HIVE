using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensX = 100;
    [SerializeField] private float sensY = 100;

    [SerializeField ]private Transform cam;
    [SerializeField ]private Transform orientation;

    private float _mouseX;
    private float _mouseY;

    private float _multiplier = 0.01f;

    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        _mouseX = Input.GetAxisRaw("Mouse X");
        _mouseY = Input.GetAxisRaw("Mouse Y");
        
        _yRotation += _mouseX * sensX * _multiplier;
        _xRotation -= _mouseY * sensY * _multiplier;
        
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);

        cam.transform.rotation = Quaternion.Euler(_xRotation,_yRotation,0);
        orientation.transform.rotation = Quaternion.Euler(0,_yRotation,0);
    }
}
