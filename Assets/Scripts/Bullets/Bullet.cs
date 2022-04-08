using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Properties")]
    public float speed = 10f;
    public float damage = 1f;
    public float timeToDie = 3f;
    public LayerMask mask;

    [HideInInspector] public bool wasShotByPlayer;

    [Header("Effects")]
    public ParticleSystem impactParticles;

    private void Start()
    {
        Destroy(gameObject, timeToDie);
    }

    Vector3 prevPos;
    protected virtual void Update()
    {
        prevPos = transform.position;
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        Vector3 dir = (transform.position - prevPos);
        RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, dir.normalized), dir.magnitude, mask);

        if(hits.Length > 0)
        {
            Collision(hits[0].collider.gameObject);
        }
    }

    protected virtual void Impact()
    {
        if(impactParticles != null)
        {
            impactParticles.transform.parent = null;
            impactParticles.transform.eulerAngles = transform.eulerAngles * -1;
            impactParticles.Play();
        }
        
        Destroy(gameObject);
    }

    public void Collision(GameObject other)
    {
        IDamagable obj = other.GetComponentInParent<IDamagable>();
        if (obj == null)
        {
            obj = other.GetComponentInChildren<IDamagable>();
        }

        if (obj != null)
        {
            if (wasShotByPlayer)
            {
                Crosshair.instance.Hit();
            }

            obj.TakeDamage(damage);
        }

        Impact();
    }
}
