using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using Kam.Utils;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Settings")]
    private int damage = 2;
    private float explosionRadius = 2f;
    private LayerMask hitMask;

    [Header("Particles")]
    private bool useParticles;
    private ParticleSystem explosion;

    [Header("Screenshake")]
    private bool useScreenShake = false;
    private float magnitude;
    private float roughness;
    private float fadeInTime;
    private float fadeOutTime;

    [Header("SFX")]
    private bool useSFX = false;
    private SFXs sfx;
    
    [Header("AddForceToRigidbodies")]
    private bool addForceToRigidbodies = true;
    private float force;
    private float upwardsForce;

    private bool exploded;

    public Explosion Initialize(Vector3 point, float explosionRadius, LayerMask mask)
    {
        transform.position = point;
        this.explosionRadius = explosionRadius;
        this.hitMask = mask;
        return this;
    }

    public Explosion SetDamage(int dmg)
    {
        this.damage = dmg;
        return this;
    }

    public Explosion SetParticles(ParticleSystem particles)
    {
        useParticles = true;
        this.explosion = particles;
        return this;
    }
    public Explosion SetScreenshake(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
    {
        useScreenShake = true;
        this.magnitude = magnitude;
        this.roughness = roughness;
        this.fadeInTime = fadeInTime;
        this.fadeOutTime = fadeOutTime;

        return this;
    }

    public Explosion SetSFX(SFXs sfx)
    {
        useSFX = true;
        this.sfx = sfx;
        return this;
    }
    
    public Explosion SetForces(float force, float upwardsForce)
    {
        addForceToRigidbodies = true;
        this.force = force;
        this.upwardsForce = upwardsForce;
        return this;
    }
    
    public void Explode()
    {
        exploded = true;

        #region Hit
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, hitMask);

        foreach (var hit in hits)
        {
            IDamageable obj = hit.GetComponentInParent<IDamageable>() ?? hit.GetComponentInChildren<IDamageable>();

            if (obj != null)
            {
                Popup.Create(hit.ClosestPoint(transform.position), damage.ToString(),KamColor.purple);
                obj.TakeDamage(damage);
            }
            
            IDirectionalDamageable directionalObj = hit.GetComponentInParent<IDirectionalDamageable>() ?? hit.GetComponentInChildren<IDirectionalDamageable>();

            if (directionalObj != null)
            {
                directionalObj.TakeDamageDirectional(damage, transform);
            }
            
            IHittable hittableObj = hit.GetComponentInParent<IHittable>() ?? hit.GetComponentInChildren<IHittable>();

            if (hittableObj != null)
            {
                Popup.Create(hit.ClosestPoint(transform.position), damage.ToString(),KamColor.purple);
                hittableObj.Hit(transform);
            }
        }
        #endregion
        
        #region Effects

        #region CameraShake
        if (useScreenShake)
        {
            CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        }
        #endregion
        
        #region AddForceToRigidbodies
        if (addForceToRigidbodies)
        {
            Collider[] hits2 = Physics.OverlapSphere(transform.position, explosionRadius, hitMask);
            foreach (var hit in hits2)
            {
                Rigidbody rb = hit.GetComponentInParent<Rigidbody>() ?? hit.GetComponentInChildren<Rigidbody>();
                
                rb?.AddExplosionForce(force,transform.position,explosionRadius,upwardsForce,ForceMode.Impulse);
            }
            
        }
        #endregion

        #region SFX

        if (useSFX)
        {
            AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(sfx));
        }
        
        #endregion
        
        #region Particles

        if (useParticles)
        {
            ParticleSystem effect = Instantiate(explosion, transform.position, Quaternion.identity);
            effect.Play();
        }
        
        #endregion
        
        #endregion
        
        Destroy(gameObject);
    }
}
