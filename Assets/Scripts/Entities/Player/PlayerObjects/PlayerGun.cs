using System;
using System.Collections;
using EZCameraShake;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class PlayerGun : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Damage the bullets will do on impact")]
    public int damage;
    [Tooltip("Conespread of bullets")]
    public float spread;
    [Tooltip("Magazine size for the gun")]
    public int maxAmmo; 
    [Tooltip("Amount of bullets that will be shot per action")]
    public int bulletsPerTap;
    
    [Tooltip("How long it will take the gun to do its next shooting action")]
    public float timeBetweenShootingActions;
    [Tooltip("How long it will take the gun to start its reload after you've stopped shooting")]
    public float reloadDelayTime; 
    [Tooltip("How long it will take the gun to reload its entire magazine")]
    public float reloadTime; 
    [Tooltip("How much the gun will space out the different bullets in its action")]
    public float timeBetweenShotsInAction;

    private float currentReloadDelayTime = 0;
    private int _currentAmmo;
    private int CurrentAmmo
    {
        get => _currentAmmo;
        set
        {
            _currentAmmo = value;
            if(currentlySelected || reloading) EventManager.Instance.Trigger("OnPlayerUpdateAmmo", _currentAmmo, maxAmmo);
        }
    }
    
    int bulletsShot;

    [Header("Energy")]
    [Tooltip("How much a single bullet costs energy wise")]
    public int energyCostPerBulletRecharge = 1;
    
    //bools 
    public bool currentlySelected { get; set; }
    bool readyToShoot; 
    bool reloading;

    //References
    [Header("Assignables")]
    public LayerMask mask;
    public Transform firePoint;
    public Bullet bullet;
    
    private Player player;
    private PlayerAim _aim;
    private ObjectPool _bulletPool;
    

    //Graphics
    [Header("Effects")]
    public GameObject shootingParticle;
    public UIGunSight.SightTypes sight = UIGunSight.SightTypes.Default;
    
    [Header("Screenshake")]
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;

    [Header("Sound Effects")]
    public bool useRechargeSfx = true;
    public SFXs rechargeSfx = SFXs.GunRecharge;
    public bool useShotSfx = true;
    public SFXs shotSfx = SFXs.PlayerShot;
    public bool useBulletSfx = false;
    public SFXs bulletSfx = SFXs.PlayerShot;

    [Header("Images")]
    public UISlotSprites gunSprites;
    public UISlotSprites skill1Sprites;
    public UISlotSprites skill2Sprites;

    public void Initialize(Player player)
    {
        this.player = player;
        CurrentAmmo = maxAmmo;
        readyToShoot = true;
        _bulletPool = ObjectPool.CreateInstance(bullet, 20);
        bullet.Parent = _bulletPool;
        _aim = new PlayerAim(player, player.shoot.firePointRight, mask, spread);
    }
    
    public void InputCheck(bool input)
    {
        bool shooting = input;
        
        if (shooting)
        {
            UIGunSight.instance.SetGunsight(sight);
            currentReloadDelayTime = 0;
            player.animator.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsShooting, true);
        }
        else
        {
            currentReloadDelayTime += Time.deltaTime;
            player.animator.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsShooting, false);
        }

        if (!reloading)
        {
            if (currentReloadDelayTime > reloadDelayTime && CurrentAmmo < maxAmmo)
            {
                Reload();
            }

            if (readyToShoot && shooting && CurrentAmmo > 0)
            {
                bulletsShot = bulletsPerTap;
                if(useShotSfx) AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(shotSfx));
                Shoot();
            }
        }
        
    }
    private void Shoot()
    {
        readyToShoot = false;

        #region Bullet Instantiation
            Bullet bul = _bulletPool.GetObject().GetComponent<Bullet>();
            bul.wasShotByPlayer = true;
            bul.damage = damage;
            bul.mask = mask;

            bul.transform.position = firePoint.position;
            bul.transform.LookAt(_aim.Aim());
            bul.trail.Clear();
        #endregion
        
        CurrentAmmo--;
        bulletsShot--;

        Invoke(nameof(ResetShot), timeBetweenShootingActions);

        if (bulletsShot > 0 && CurrentAmmo > 0)
        {
            Invoke(nameof(Shoot), timeBetweenShotsInAction);
        }
        
        #region Effects
            CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
                
            GameObject particle = Instantiate(shootingParticle, firePoint.position, firePoint.rotation);
            Destroy(particle, 1f);
                
            if(useBulletSfx) AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(bulletSfx));
        #endregion
    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        reloadCoroutine = StartCoroutine(nameof(ReloadingCoroutine));
    }

    private bool _playedSFX;
    private Coroutine reloadCoroutine;
    private IEnumerator ReloadingCoroutine()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
        
            int recharge = Mathf.CeilToInt(maxAmmo / reloadTime * Time.deltaTime);
            float energyCost = recharge * energyCostPerBulletRecharge;
            if (!player.energy.TakeEnergy(energyCost))
            {
                StopReload();
            }
            
            if (!_playedSFX && reloading)
            {
                if(useRechargeSfx) AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(rechargeSfx));
                _playedSFX = true;
            }

            CurrentAmmo += recharge;

            if (CurrentAmmo >= maxAmmo)
            {
                CurrentAmmo = maxAmmo;
                _playedSFX = false;
                StopReload();
            }
        }
    }
    
    private void StopReload()
    {
        reloading = false;
        currentReloadDelayTime = 0;
        StopCoroutine(reloadCoroutine);
    }
}