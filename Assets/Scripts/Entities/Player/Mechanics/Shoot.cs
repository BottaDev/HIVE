using UnityEngine;
using UnityEngine.Serialization;

public class Shoot : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Player player;

    [Header("Energy")]
    public int energyCostPerBulletRecharge = 1;

    [Header("Gun")]
    public int damage = 5;
    public float fireRate = 15;
    public float gunCd = 1f;
    public bool reloading;

    [Header("Ammunition")]
    public int maxAmmo = 100;
    public float totalReloadTime = 1f;
    

    [Header("Objects")]
    public LayerMask mask;
    public Bullet bullet;

    [FormerlySerializedAs("_firePoint")] public Transform firePoint;
    private ObjectPool _bulletPool;

    private int _currentAmmo;
    private float _currentCd;

    private float _nextShoot;

    private bool _playedSFX;
    //Get whatever information you need for this script
    private bool Shooting => player.input.Shooting;

    private int CurrentAmmo
    {
        get => _currentAmmo;
        set
        {
            _currentAmmo = value;
            EventManager.Instance.Trigger(EventManager.Events.OnPlayerUpdateAmmo, _currentAmmo, maxAmmo);
        }
    }

    private void Start()
    {
        CurrentAmmo = maxAmmo;

        _bulletPool = ObjectPool.CreateInstance(bullet, 20);
        bullet.Parent = _bulletPool;
    }

    private void Update()
    {
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
            if(CurrentAmmo < maxAmmo)
            {
                ReloadGun();
            }
        }
        else
        {
            _currentCd += Time.deltaTime;
        }
    }

    private void ShootAction()
    {
        if (CurrentAmmo > 0)
        {
            reloading = false;
            _currentCd = 0;

            AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerShot));
            Bullet bul = _bulletPool.GetObject().GetComponent<Bullet>();


            bul.wasShotByPlayer = true;
            bul.damage = damage;
            bul.mask = mask;

            bul.transform.position = firePoint.position;
            bul.transform.LookAt(player.aim.Point);

            bul.trail.Clear();
            CurrentAmmo -= 1;

            if (CurrentAmmo <= 0)
            {
                CurrentAmmo = 0;
            }
        }
    }

    private void ReloadGun()
    {
        int recharge = Mathf.CeilToInt(maxAmmo / totalReloadTime * Time.deltaTime);
        float energyCost = recharge * energyCostPerBulletRecharge;
        if (!player.energy.TakeEnergy(energyCost)) return;


        if (!_playedSFX && reloading)
        {
            AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.GunRecharge));
            _playedSFX = true;
        }
        
        reloading = true;
        CurrentAmmo += recharge;

        if (CurrentAmmo >= maxAmmo)
        {
            CurrentAmmo = maxAmmo;
            reloading = false;
            _playedSFX = false;
        }
    }
}