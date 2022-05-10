using UnityEngine;
using UnityEngine.Serialization;

public class ThirdCameraController : MonoBehaviour
{
    [Header("Clamp")]
    private const float MinY = -70f;
    private const float MaxY = 70;
    [Header("Assignables")]
    public LayerMask mask;
    [FormerlySerializedAs("_lookAt")] public Transform lookAt;
    [FormerlySerializedAs("_camTransform")] public Transform camTransform;
    public PlayerInput input;

    [Header("Parameters")]
    public float mouseSensitivity = 1f;
    private float _currentX;
    private float _currentY;


    private readonly float _distance = 5f;

    private void Update()
    {
        _currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        _currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        _currentY = Mathf.Clamp(_currentY, MinY, MaxY);
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -_distance);
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);
        transform.position = lookAt.position + rotation * dir;

        transform.LookAt(lookAt.position);

        //Collisions
        Vector3 localPosCam = Vector3.zero;
        if (Physics.Linecast(lookAt.position, transform.position, out RaycastHit hit, mask))
        {
            float distHit = Vector3.Distance(hit.point, transform.position) + 2;
            localPosCam = Vector3.forward * distHit;
        }

        camTransform.localPosition = localPosCam;
    }
}