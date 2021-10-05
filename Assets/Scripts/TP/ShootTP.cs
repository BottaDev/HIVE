using UnityEngine;

public class ShootTP : MonoBehaviour
{
    [Header("Gun")]
    public float fireRate = 15;
    public float gunCd = 1f;
    private float _nextShoot;
    private bool _reloading;
    
    [Header("Ammunition")]
    public int maxAmmo = 100;
    public int reloadAmmount = 1;
    public int ammoCost = 1;
    private int _currentAmmo;
    
    [Header("Objects")]
    public LayerMask mask;
    public GameObject bullet;
    
    private Transform _firePoint;
    private Vector3 _mouseWorldPosition;
    
    private Camera _cam;
    private AmmoBar _ammoBar;

    private void Start()
    {
        _ammoBar = FindObjectOfType<AmmoBar>();
        _firePoint = GameObject.Find("FirePoint").GetComponent<Transform>();
        _cam = Camera.main;
        
        _ammoBar.SetMaxAmmo(maxAmmo, _currentAmmo);
    }
    
    private void Update()
    {
        if (_currentAmmo <= 0)
        {
            Invoke(nameof(ReloadGun), gunCd);
            return;
        }
        
        _mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = _cam.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mask))
        {
            _mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }


        if (Input.GetButton("Fire1") && Time.time >= _nextShoot)
        {
            _reloading = false;
            _nextShoot = Time.time + 1 / fireRate;
            Shoot();   
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
    
    private void Shoot()
    {
        Vector3 aimDir = (_mouseWorldPosition - _firePoint.position).normalized;        
        Instantiate(bullet, _firePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
        
        _currentAmmo -= ammoCost;
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
}
