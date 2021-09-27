 using UnityEngine;

public class ThirdCameraController : MonoBehaviour
{
    private const float _minY = -70f;
    private const float _maxY = 70;
    
    public Transform lookAt;
    public Transform camTransform;
    public LayerMask mask;

    private float _distance = 5f;
    private float _currentX;
    private float _currentY;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _currentX += Input.GetAxis("Mouse X");
        _currentY -= Input.GetAxis("Mouse Y");

        _currentY = Mathf.Clamp(_currentY, _minY, _maxY);
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -_distance);
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);
        transform.position = lookAt.position + rotation * dir;
        transform.LookAt(lookAt.position);

        RaycastHit hit;
        Vector3 localPosCam = Vector3.zero;
        if (Physics.Linecast(lookAt.position, transform.position, out hit, mask))
        {
            float distHit = Vector3.Distance(hit.point, transform.position);
            localPosCam = Vector3.forward * distHit;
        }

        camTransform.localPosition = localPosCam;
    }
}
