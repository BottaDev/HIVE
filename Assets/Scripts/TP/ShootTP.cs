using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTP : MonoBehaviour
{
    public float damage = 1;
    public float range = 100;
    public float fireRate = 15;

    public Camera cam;
    public PlayerTP playerTP;

    private float _nextShoot = 0;

    public ParticleSystem shootEffect;
    public GameObject impactEffect;

    public GameObject bullet;
    public Transform firePoint;
    public Transform crosshair;
    public LayerMask mask;
    Vector3 mouseWorldPosition;

    private void Update()
    {
        mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = cam.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mask))
        {
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }


        if (Input.GetButton("Fire1") && Time.time >= _nextShoot)
        {
            _nextShoot = Time.time + 1 / fireRate;
            ShootGO();   
        }
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            TargetTP target = hit.transform.GetComponent<TargetTP>();
            if (target != null)
                target.TakeDamage(damage);

            GameObject particle = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(particle, 2);
        }
    }

    private void ShootGO()
    {
        Vector3 aimDir = (mouseWorldPosition - firePoint.position).normalized;        
        Instantiate(bullet, firePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
    }
}
