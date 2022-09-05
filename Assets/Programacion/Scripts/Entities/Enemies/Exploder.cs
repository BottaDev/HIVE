using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using UnityEngine;

public class Exploder : AI
{
    [Header("Exploder Parameters")] 
    public int explosionDamage = 5;
    public float explosionRadius = 2;
    public LayerMask explosionHitMask;
    public float engageSpeed = 12f;
    public float engageDistance = 8.5f;
    public float explodeDistance = 5f;
    [Tooltip("Time it takes to explode")] public float explodeWarningTime = 0.8f;
    [Tooltip("Time it takes to start engaging")] public float waitDuration = 0.8f;

    [Header("Objects")] 
    public ParticleSystem explosion;
    
    private bool _engaged;
    
    protected override void Update()
    {
        base.Update();
        if (dying) return;

        if (_playerDetected)
            ChasePlayer();
    }
    
    private void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= engageDistance && !_engaged && !_agent.isOnOffMeshLink)
        {
            StartCoroutine(EngagePlayer());
            return;
        }
        
        MoveToPosition(_player.transform.position);
        
        if (distance <= explodeDistance && _engaged)
            Attack();
    }

    private IEnumerator EngagePlayer()
    {
        CurrentSpeed = engageSpeed;
        _agent.isStopped = true;
        yield return new WaitForSeconds(waitDuration);
        _agent.isStopped = false;

        _engaged = true;
    }
    
    protected override void Attack()
    {
        Destroy(gameObject, explodeWarningTime);   
    }

    private void OnDestroy()
    {
        Explosion exp = ExplosionManager.i.NewExplosion(transform.position, explosionRadius, explosionHitMask);
        exp.SetDamage(explosionDamage);
        exp.SetParticles(explosion);
        exp.SetSFX(SFXs.Explosion);
        exp.SetScreenshake(25, 4, 0.1f, 1f);
        exp.SetForces(10,10);
        exp.Explode();
        anim.SetTrigger("Boom");
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeDistance);
        
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, engageDistance);
    }
}
