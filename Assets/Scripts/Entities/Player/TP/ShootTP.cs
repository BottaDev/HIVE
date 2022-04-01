using System.Collections;
using UnityEngine;

public class ShootTP : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private PlayerInput input;

    //Get whatever information you need for this script
    private bool shooting { get { return input.shooting; } }

    [Header("Gun")]
    public float fireRate = 15;
    public float gunCd = 1f;
    private float currentCD;
    private float _nextShoot;
    public bool _reloading;
    
    [Header("Ammunition")]
    public int maxAmmo = 100;
    public float totalReloadTime = 1f;
    public int ammoCost = 1;
    [SerializeField] private int _currentAmmo;
    private int currentAmmo { get { return _currentAmmo; } set { _currentAmmo = value; _ammoBar.UpdateFillAmount(_currentAmmo); } }
    
    [Header("Objects")]
    public LayerMask mask;
    public GameObject bullet;
    
    public Transform _firePoint;
    private Vector3 _mouseWorldPosition;
    
    private Camera _cam;
    public AmmoBar _ammoBar;

    private void Start()
    {
        _cam = Camera.main;

        _currentAmmo = maxAmmo;
        _ammoBar.SetMaxAmmo(maxAmmo);
    }

    private void Update()
    {
        _mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = _cam.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mask))
        {
            _mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }


        if (shooting)
        {
            if (Time.time >= _nextShoot)
            {
                _nextShoot = Time.time + 1 / fireRate;
                Shoot();
            }
        }

        if (currentCD > gunCd)
        {
            ReloadGun();
        }
        else
        {
            currentCD += Time.deltaTime;
        }
    }
    private void OnDrawGizmos()
    {
        Vector3 aimDir = _mouseWorldPosition - _firePoint.position;
        Debug.DrawLine(_firePoint.position, aimDir, Color.red);
    }
    private void Shoot()
    {
        if (currentAmmo > 0)
        {
            _reloading = false;
            currentCD = 0;

            Vector3 aimDir = (_mouseWorldPosition - _firePoint.position).normalized;
            Instantiate(bullet, _firePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));

            currentAmmo -= ammoCost;

            if (currentAmmo <= 0)
            {
                currentAmmo = 0;
            } 
        }
    }

    private void ReloadGun()
    {
        _reloading = true;
        currentAmmo += Mathf.CeilToInt((maxAmmo / totalReloadTime) * Time.deltaTime);

        if (currentAmmo >= maxAmmo)
        {
            currentAmmo = maxAmmo;
            _reloading = false;
        }
    }
}
