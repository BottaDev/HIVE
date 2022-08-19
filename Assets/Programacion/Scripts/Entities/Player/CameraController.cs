using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

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
    }

    private void LateUpdate()
    {

        cameraCenter.transform.position = new Vector3(
            character.transform.position.x, 
            character.transform.position.y + yOffset, 
            character.transform.position.z);

        sensitivity = SettingsManager.mouseSensitivity;
        float tempSensitivity = UIPauseMenu.paused ? 0 : SettingsManager.mouseSensitivity;
        Quaternion rot = Quaternion.Euler(
            cameraCenter.transform.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * tempSensitivity / 2,
            cameraCenter.transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * tempSensitivity,
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

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(CameraController))]
public class KamCustomEditor_CameraController : KamCustomEditor
{
    private CameraController editorTarget;
    private void OnEnable()
    {
        editorTarget = (CameraController)target;
    }
    
    public override void GameDesignerInspector()
    {
        EditorGUILayout.LabelField("General Parameters", EditorStyles.centeredGreyMiniLabel);
        
        
        
        editorTarget.yOffset = EditorGUILayout.FloatField(
            new GUIContent(
                "Height Offset",
                "This is the offset that the camera will use in the Y axis. To visualize this, you can set the Y coordinate of this object in the inspector."),
            editorTarget.yOffset);
        
        editorTarget.sensitivity = EditorGUILayout.FloatField(
            new GUIContent(
                "Sensitivity",
                "The mouse sensitivity with which the camera is controlled"),
            editorTarget.sensitivity);
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Zoom Parameters", EditorStyles.centeredGreyMiniLabel);
        
        editorTarget.scrollSensitivity = EditorGUILayout.FloatField(
            new GUIContent(
                "Sensitivity",
                "Zoom sensitivity."),
            editorTarget.scrollSensitivity);
        
        
        var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
        EditorGUILayout.LabelField(
            new GUIContent(
            "Zoom",
            "This is the force with which the player throws the grenade."), style, GUILayout.ExpandWidth(true));

        
        EditorGUILayout.BeginHorizontal();
        editorTarget.zoomDefault = EditorGUILayout.FloatField(
            new GUIContent(
                "Default",
                "Default zoom amount"), editorTarget.zoomDefault);

        editorTarget.zoomMin = EditorGUILayout.FloatField(
            new GUIContent(
                "Min",
                "Minimum zoom proximity"), editorTarget.zoomMin);
        
        editorTarget.zoomMax = EditorGUILayout.FloatField(
            new GUIContent(
                "Max",
                "Maximum zoom proximity"), editorTarget.zoomMax);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Collision Parameters", EditorStyles.centeredGreyMiniLabel);
        
        editorTarget.cameraCollisionMask = EditorGUILayout.MaskField(
            new GUIContent("Collision Mask",
                "Layers that the camera will collide with."),
            InternalEditorUtility.LayerMaskToConcatenatedLayersMask(editorTarget.cameraCollisionMask), InternalEditorUtility.layers);
        
        editorTarget.minimumCollisionProximity = EditorGUILayout.FloatField(
            new GUIContent(
                "Min Collision Proximity",
                "The closest the camera can be to the player during collision"), editorTarget.minimumCollisionProximity);
        
        editorTarget.collisionSensitivity = EditorGUILayout.FloatField(
            new GUIContent(
                "Collision Sensitivity",
                "The max distance from the camera a collision can be detected at."), editorTarget.collisionSensitivity);
    }
}
#endif
#endregion
