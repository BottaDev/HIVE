using System;
using UnityEngine;
public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Player player;
    public LayerMask aimingMask;
    public Transform firePoint;
    
    [SerializeField] private bool drawGizmos;
    /// <summary>
    /// The in-world aiming point for the player
    /// </summary>
    private Vector3 _point;
    private Camera _cam;
    private bool _success;
    private Ray _ray;
    
    public Vector3 Point { get => _point; set => _point = value; }
    public bool Aim { get => _success; set => _success = value; }
    
    private void Start()
    {
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        Aim = false;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray screenRay = _cam.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(screenRay, out RaycastHit raycastHit, 999f, aimingMask))
        {
            //Screen ray is colliding with something, now check what point a ray would collide with from the player transform
            
            Vector3 point = raycastHit.point;
            Ray playerTramsformRay = new Ray(player.transform.position, (point - player.transform.position).normalized);
            
            if (Physics.Raycast(playerTramsformRay, out RaycastHit raycastHitTwo, 999f, aimingMask))
            {
                Point = raycastHitTwo.point;
                Aim = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(firePoint.position, Point);
        }
    }
}
