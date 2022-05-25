using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Assignables")]
    public Rigidbody rb;
    public ParticleSystem impactParticles;
    public TrailRenderer trail;

    [Header("Screenshake")]
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;

    [Header("Settings")]
    public bool setParticleParametersInCode;
    

    private float timeDelay;
    private float timeCounter;
    private int damage = 2;
    private float explosionRadius = 2f;
    private LayerMask hitMask;
    
    private bool explodeOnContact = true;
    
    
    private bool addForceToRigidbodies = true;
    private float force;
    private float upwardsForce;

    private bool exploded;
    private void Update()
    {
        if (!exploded)
        {
            timeCounter += Time.deltaTime;

            if(timeCounter > timeDelay)
            {
                Explode();
            }
        }
    }

    public Grenade SetParameters(float radius, int damage, float timeDelay, bool explodeOnContact, LayerMask hitMask)
    {
        explosionRadius = radius;
        this.damage = damage;
        this.hitMask = hitMask;
        this.timeDelay = timeDelay;
        this.explodeOnContact = explodeOnContact;
        return this;
    }

    public Grenade SetAddForceToRigidbodies(bool add, float force, float upwardsForce)
    {
        addForceToRigidbodies = add;
        this.force = force;
        this.upwardsForce = upwardsForce;

        return this;
    }

    void Explode()
    {
        Debug.Log("Art is an Explosion!");
        exploded = true;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, hitMask);

        foreach (var hit in hits)
        {
            IDamageable obj = hit.GetComponentInParent<IDamageable>() ?? hit.GetComponentInChildren<IDamageable>();

            if (obj != null)
            {
                obj.TakeDamage(damage);
            }
        }
        
        #region Effects
        ParticleSystem effect = Instantiate(impactParticles, transform.position, transform.rotation);
        
        if(setParticleParametersInCode)
        {
            ParticleSystem.MainModule main = effect.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(20*explosionRadius);
            main.startSize = new ParticleSystem.MinMaxCurve(0.1f*explosionRadius);
            ParticleSystem.EmissionModule emission = effect.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0f,100 * (int)explosionRadius)) ;
            
            ParticleSystem.ShapeModule shape = effect.shape;
            //shape.radius = radius;
        }
        
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        if (addForceToRigidbodies)
        {
            Collider[] hits2 = Physics.OverlapSphere(transform.position, explosionRadius, hitMask);
            foreach (var hit in hits2)
            {
                Rigidbody rb = hit.GetComponentInParent<Rigidbody>() ?? hit.GetComponentInChildren<Rigidbody>();
                
                rb?.AddExplosionForce(force,transform.position,explosionRadius,upwardsForce,ForceMode.Impulse);
            }
            
        }

        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.Explosion));
        effect.Play();
        #endregion
        
        Destroy(gameObject);
    }

    IEnumerator DestroyCoroutine(ParticleSystem effect, float time)
    {
        yield return new WaitForSeconds(time);
        
        Destroy(effect);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explodeOnContact)
        {
            if (hitMask.CheckLayer(collision.gameObject.layer))
            {
                Explode();
            }
        }
        
    }
}
