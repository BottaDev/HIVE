using System;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public float damage = 1;
    public float range = 100;
    public float fireRate = 15;
    public float cooldownRate = 0.5f;

    public Camera cam;

    public ParticleSystem shootEffect;
    public GameObject impactEffect;
    
    private float _nextShoot = 0;
    private bool _inCooldown;

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= _nextShoot)
        {
            _nextShoot = Time.time + 1 / fireRate;
            Shooter();
        }
        else
        {
            _inCooldown = true;
        }
    }

    private void Shooter()
    {
        shootEffect.Play();

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
                target.TakeDamage(damage);

            GameObject particle = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(particle, 2);
        }
    }
}
