 using UnityEngine;

public class ThirdCameraController : MonoBehaviour
{
    [Header("Assignables")]
    public LayerMask mask;
    public Transform _lookAt;
    public Transform _camTransform;
    public PlayerInput input;

    [Header("Parameters")]
    public float mouseSensitivity = 1f;



    private float _distance = 5f;
    
    [Header("Clamp")]   
    private const float _minY = -70f;
    private const float _maxY = 70;
    private float _currentX;
    private float _currentY;
    
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        _currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        _currentY = Mathf.Clamp(_currentY, _minY, _maxY);
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -_distance);
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);
        transform.position = _lookAt.position + rotation * dir;

        transform.LookAt(_lookAt.position);

        //Collisions
        RaycastHit hit;
        Vector3 localPosCam = Vector3.zero;
        if (Physics.Linecast(_lookAt.position, transform.position, out hit, mask))
        {
            float distHit = Vector3.Distance(hit.point, transform.position)+2;
            localPosCam = Vector3.forward * distHit;
        }

        _camTransform.localPosition = localPosCam;
    }
}
