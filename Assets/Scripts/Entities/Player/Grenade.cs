using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public Rigidbody rb;
    public ParticleSystem impactParticles;
    public TrailRenderer trail;


    private float timeDelay;
    private float timeCounter;
    private float damage = 2f;
    private float explosionRadius = 2f;
    private LayerMask hitMask;
    
    private bool explodeOnContact = true;
    
    
    private bool addForceToRigidbodies = true;
    private float force;
    private float upwardsForce;
    private void Update()
    {
        timeCounter += Time.deltaTime;

        if(timeCounter > timeDelay)
        {
            Explode();
        }
    }

    public Grenade SetParameters(float radius, float damage, float timeDelay, bool explodeOnContact, LayerMask hitMask)
    {
        explosionRadius = radius;
        this.damage = damage;
        this.hitMask = hitMask;
        this.timeDelay = timeDelay;
        this.explodeOnContact = explodeOnContact;

        ParticleSystem.MainModule main = impactParticles.main;
        main.startSpeed = new ParticleSystem.MinMaxCurve(20*radius);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f*radius);
        ParticleSystem.EmissionModule emission = impactParticles.emission;
        emission.SetBurst(0, new ParticleSystem.Burst(0f,100 * (int)radius)) ;
        
        ParticleSystem.ShapeModule shape = impactParticles.shape;
        //shape.radius = radius;

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
        ParticleSystem effect = Instantiate(impactParticles, transform.position, transform.rotation);
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, hitMask);

        foreach (var hit in hits)
        {
            IDamageable obj = hit.GetComponentInParent<IDamageable>() ?? hit.GetComponentInChildren<IDamageable>();

            if (obj != null)
            {
                obj.TakeDamage(damage);
            }

            
        }
        
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
