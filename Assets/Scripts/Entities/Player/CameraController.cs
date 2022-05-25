using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraController : MonoBehaviour
{
    public GameObject character;
    public GameObject cameraCenter;
    public CameraShaker shaker;
    public Camera cam;
    public float yOffset = 1f;
    public float sensitivity = 1f;
    

    public float scrollSensitivity = 5f;
    public float scrollDampening = 6f;
    
    public float zoomMin = 3.5f;
    public float zoomMax = 15f;
    public float zoomDefault = 10f;
    private float _zoomDistance;

    public LayerMask cameraCollisionMask;
    public float minimumCollisionProximity = 1f;
    public float collisionSensitivity = 4.5f;

    private RaycastHit _camHit;
    private Vector3 _camDist;
    private Vector3 _camLocalPos;
    private Vector3 _camPos;
    private void Start()
    {
        _camDist = cam.transform.localPosition;
        _zoomDistance = zoomDefault;
        _camDist.z = _zoomDistance;
        CameraShaker.Instance.StopAllCoroutines();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        cameraCenter.transform.position = new Vector3(
            character.transform.position.x, 
            character.transform.position.y + yOffset, 
            character.transform.position.z);
        
        Quaternion rot = Quaternion.Euler(
            cameraCenter.transform.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity / 2,
            cameraCenter.transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * sensitivity,
            cameraCenter.transform.rotation.eulerAngles.z);

        cameraCenter.transform.rotation = rot;

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            var scrollAmount = scroll * scrollSensitivity;
            scrollAmount *= _zoomDistance * 0.3f;

            _zoomDistance += -scrollAmount;

            _zoomDistance = Mathf.Clamp(_zoomDistance, zoomMin, zoomMax);
        }

        if (Math.Abs(_camDist.z - (-_zoomDistance)) > 0.01f)
        {
            _camDist.z = Mathf.Lerp(_camDist.z, -_zoomDistance, Time.deltaTime * scrollDampening);
        }

        cam.transform.localPosition = _camDist;

        GameObject obj = new GameObject();
        obj.transform.SetParent(cam.transform.parent);
        obj.transform.localPosition = new Vector3(
            cam.transform.localPosition.x, 
            cam.transform.localPosition.y,
            cam.transform.localPosition.z - collisionSensitivity);

        if (Physics.Linecast(cameraCenter.transform.position, obj.transform.position, out _camHit,cameraCollisionMask))
        {
            cam.transform.position = _camHit.point;

            var localPos = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y,
                cam.transform.localPosition.z + collisionSensitivity);
            cam.transform.localPosition = localPos;
        }

        Destroy(obj);

        if (cam.transform.localPosition.z > -minimumCollisionProximity)
        {
            cam.transform.localPosition =
                new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, -minimumCollisionProximity);
        }

        _camLocalPos = cam.transform.localPosition;
        _camPos = cam.transform.position;
    }
}
