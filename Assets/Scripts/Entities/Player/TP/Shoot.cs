using UnityEngine;
using UnityEngine.Serialization;

public class Shoot : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Player player;

    [Header("Gun")]
    public float damage = 5;
    public float fireRate = 15;
    public float gunCd = 1f;
    public bool reloading;

    [Header("Ammunition")]
    public int maxAmmo = 100;
    public float totalReloadTime = 1f;
    public int ammoCost = 1;

    [Header("Objects")]
    public LayerMask mask;
    public Bullet bullet;

    [FormerlySerializedAs("_firePoint")] public Transform firePoint;
    [FormerlySerializedAs("_ammoBar")] public AmmoBar ammoBar;
    private ObjectPool _bulletPool;

    private Camera _cam;
    private int _currentAmmo;
    private float _currentCd;
    private Vector3 _mouseWorldPosition;
    private float _nextShoot;

    //Get whatever information you need for this script
    private bool Shooting => player.input.Shooting;

    private int CurrentAmmo
    {
        get => _currentAmmo;
        set
        {
            _currentAmmo = value;
            ammoBar.UpdateFillAmount(_currentAmmo);
        }
    }

    private void Start()
    {
        _cam = Camera.main;

        _currentAmmo = maxAmmo;
        ammoBar.SetMaxAmmo(maxAmmo);

        _bulletPool = ObjectPool.CreateInstance(bullet, 20);
        bullet.Parent = _bulletPool;
    }

    private void Update()
    {
        _mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = _cam.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mask))
        {
            _mouseWorldPosition = raycastHit.point;
        }


        if (Shooting)
        {
            player.animator.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsShooting, true);
            if (Time.time >= _nextShoot)
            {
                _nextShoot = Time.time + 1 / fireRate;
                ShootAction();
            }
        }
        else
        {
            player.animator.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsShooting, false);
        }

        if (_currentCd > gunCd)
        {
            ReloadGun();
        }
        else
        {
            _currentCd += Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 aimDir = _mouseWorldPosition - firePoint.position;
        Debug.DrawLine(firePoint.position, aimDir, Color.red);
    }

    private void ShootAction()
    {
        if (CurrentAmmo > 0)
        {
            reloading = false;
            _currentCd = 0;

            Bullet bul = _bulletPool.GetObject().GetComponent<Bullet>();


            bul.wasShotByPlayer = true;
            bul.damage = damage;

            bul.transform.position = firePoint.position;
            bul.transform.LookAt(_mouseWorldPosition);

            bul.trail.Clear();
            CurrentAmmo -= ammoCost;

            if (CurrentAmmo <= 0)
            {
                CurrentAmmo = 0;
            }
        }
    }

    private void ReloadGun()
    {
        reloading = true;
        CurrentAmmo += Mathf.CeilToInt(maxAmmo / totalReloadTime * Time.deltaTime);

        if (CurrentAmmo >= maxAmmo)
        {
            CurrentAmmo = maxAmmo;
            reloading = false;
        }
    }
}