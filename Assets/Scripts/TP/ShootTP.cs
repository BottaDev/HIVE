using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTP : MonoBehaviour
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
    public PlayerTP playerTP;

    private float _nextShoot = 0;

    public ParticleSystem shootEffect;
    public GameObject impactEffect;

    public GameObject bullet;
    public Transform firePoint;
    public Transform crosshair;
    public LayerMask mask;
    Vector3 mouseWorldPosition;
    
    private int _currentAmmo;
    private AmmoBar _ammoBar;
    private bool _reloading;

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
            _reloading = false;
            _nextShoot = Time.time + 1 / fireRate;
            ShootGO();   
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
        
        _currentAmmo--;
    }
}
