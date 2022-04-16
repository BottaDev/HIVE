using UnityEngine;
public class PlayerAim : MonoBehaviour
{
    public LayerMask aimingMask;
    
    /// <summary>
    /// The in-world aiming point for the player
    /// </summary>
    private Vector3 _point;
    private Camera _cam;
    private bool _success;
    private Ray _ray;
    
    public Vector3 Point { get => _point; set => _point = value; }
    public bool Aim { get => _success; set => _success = value; }
    public Ray Ray { get => _ray; set => _ray = value; }
    
    private void Start()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        Aim = false;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray = _cam.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(Ray, out RaycastHit raycastHit, 999f, aimingMask))
        {
            Point = raycastHit.point;
            Aim = true;
        }
    }
}
