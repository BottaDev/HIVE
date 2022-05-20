using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAim
{
    //References
    private Player player;
    private Transform firePoint;

    //Settings
    private LayerMask aimingMask;
    private float spread;

    public PlayerAim(Player player, Transform firePoint, LayerMask aimingMask, float spread)
    {
        this.player = player;
        this.firePoint = firePoint;
        this.aimingMask = aimingMask;
        this.spread = spread;
    }
    
    
    public Vector3 Aim()
    {
        Camera _cam = Camera.main;
        
        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 direction = _cam.transform.forward + _cam.transform.right * x + _cam.transform.up * y;

        if (Physics.Raycast(_cam.transform.position, direction, out RaycastHit raycastHit, 999f, aimingMask))
        {
            //Screen ray is colliding with something, now check what point a ray would collide with from the player transform

            Ray playerTramsformRay = new Ray(player.transform.position, (raycastHit.point - player.transform.position).normalized);
            
            if (Physics.Raycast(playerTramsformRay, out RaycastHit raycastHitTwo, 999f, aimingMask))
            {
                return raycastHitTwo.point;
            }
        }
        
        return _cam.transform.forward * 999f;
    }

    public void DrawGizmos(Vector3 point)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(firePoint.position, point);
    }
}
