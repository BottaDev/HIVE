using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KamShooter : MonoBehaviour
{
    Transform _player;

    [Header("Shoot parameters")]
    public Transform shootingPoint;
    public GameObject bulletPrefab;
    public float shootEvery;

    public bool paused => UIPauseMenu.paused;
    
    [Header("Events")]
    public UnityEvent onStartShooting;
    public UnityEvent onShoot;
    public UnityEvent onStopShooting;

    private void Start()
    {
        EventManager.Instance.Subscribe("SendPlayerReference", GetPlayerReference);
        EventManager.Instance.Trigger("NeedsPlayerReference");
        EventManager.Instance.Unsubscribe("SendPlayerReference", GetPlayerReference);
    }
    
    private void GetPlayerReference(params object[] p)
    {
        _player = ((Player)p[0]).transform;
    }

    //Called in animation event
    public void StartAttacking()
    {
        if (!shooting)
        {
            shootCoroutine = StartCoroutine(ShootCoroutine());
            
            onStartShooting?.Invoke();
        }
    }
    
    //Called in animation event
    public void StopAttacking()
    {
        if (shooting)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
            
            onStopShooting?.Invoke();
        }
    }
    
    //Called in animation event
    public void Shoot()
    {
        if (paused) return;
        
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        bullet.transform.LookAt(_player.transform.position);
        
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EnemyShot));
    }

    private Coroutine shootCoroutine;
    private bool shooting => shootCoroutine != null;

    IEnumerator ShootCoroutine()
    {
        while (true)
        {
            onShoot?.Invoke();
            yield return new WaitForSeconds(shootEvery);
        }
    }
}
