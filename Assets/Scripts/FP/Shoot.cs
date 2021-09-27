using System;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Shoot : MonoBehaviour
{
    [Header("Gun")]
    public float damage = 1;
    public float range = 100;
    public float fireRate = 15;
    public float gunCd = 1f;
    [Header("Ammunition")] 
    public int maxAmmo = 100;
    public int reloadAmmount = 1;
    
    [Header("Objects")]
    public Camera cam;
    public ParticleSystem shootEffect;
    public GameObject impactEffect;

    public int _currentAmmo;
    private float _nextShoot = 0;
    private AmmoBar _ammoBar;
    private bool _reloading;

    private void Awake()
    {
        _currentAmmo = maxAmmo;
    }

    private void Start()
    {
        _ammoBar = FindObjectOfType<AmmoBar>();
        
        _ammoBar.SetMaxAmmo(maxAmmo, _currentAmmo);
    }

    private void Update()
    {
        if (_currentAmmo <= 0)
        {
            Invoke(nameof(ReloadGun), gunCd);
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= _nextShoot)
        {
            _reloading = false;
            _nextShoot = Time.time + 1 / fireRate;
            Shooter();
        }
        else if (!Input.GetButton("Fire1"))
        {
            if (_reloading)
                Invoke(nameof(ReloadGun), gunCd);
            else
                ReloadGun();   
        }
        
        UpdateAmmoBar();
    }

    private void ReloadGun()
    {
        if (_currentAmmo >= maxAmmo)
        {
            _currentAmmo = maxAmmo;
            _reloading = false;
            return;
        }

        _reloading = true;

        _currentAmmo += reloadAmmount;
    }
    
    private void UpdateAmmoBar()
    {
        _ammoBar.SetAmmo(_currentAmmo);
    }
    
    private void Shooter()
    {
        shootEffect.Play();

        _currentAmmo--;
        
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            StaticEnemy target = hit.transform.GetComponent<StaticEnemy>();
            if (target != null)
                target.TakeDamage(damage);

            GameObject particle = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(particle, 2);
        }
    }
}
