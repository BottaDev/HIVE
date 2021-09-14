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

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= _nextShoot)
        {
            _nextShoot = Time.time + 1 / fireRate;
            Shoot();   
        }
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
            TargetTP target = hit.transform.GetComponent<TargetTP>();
            if (target != null)
                target.TakeDamage(damage);

            GameObject particle = (GameObject)Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(particle, 2);
        }
        else
            targetPoint = ray.GetPoint(1000);
    }
}
