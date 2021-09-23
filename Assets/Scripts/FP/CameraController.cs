using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _cam;
    [SerializeField] private Transform _player;

    private float _mouseX;
    private float _mouseY;

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
        
        _yRotation += _mouseX;
        _xRotation -= _mouseY;
        
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);

        _cam.transform.rotation = Quaternion.Euler(_xRotation,_yRotation,0);
        _player.transform.rotation = Quaternion.Euler(0,_yRotation,0);
    }
}
