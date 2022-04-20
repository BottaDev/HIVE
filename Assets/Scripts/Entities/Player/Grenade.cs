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

    private void Update()
    {
        timeCounter += Time.deltaTime;

        if(timeCounter > timeDelay)
        {
            Explode();
        }
    }

    public void SetParameters(float radius, float damage, float timeDelay, bool explodeOnContact, LayerMask hitMask)
    {
        explosionRadius = radius;
        this.damage = damage;
        this.hitMask = hitMask;
        this.timeDelay = timeDelay;
        this.explodeOnContact = explodeOnContact;
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
