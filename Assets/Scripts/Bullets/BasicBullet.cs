using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    [Header("Properties")]
    public float speed = 10f;
    public float damage = 1f;
    public float timeToDie = 3f;
    public float distance = 0.7f;
    public LayerMask mask;
    public RaycastHit hit;
    
    [Header("Effects")]
    public ParticleSystem impactParticles;

    private void Start()
    {
        Destroy(gameObject, timeToDie);
    }

    protected virtual void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        
        if(Physics.Raycast(transform.position, transform.forward, out hit, distance, mask))
            Impact();
    }

    protected virtual void Impact()
    {
        impactParticles.transform.parent = null;
        impactParticles.transform.eulerAngles = transform.eulerAngles * -1;
        impactParticles.Play();
        
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        
        Impact();
    }
}
