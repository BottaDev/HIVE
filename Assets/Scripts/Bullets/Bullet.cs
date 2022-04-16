using System;
using UnityEngine;

public sealed class Bullet : PoolableObject
{
    [Header("Properties")]
    public float speed = 10f;
    public float damage = 1f;
    public float timeToDie = 3f;
    public LayerMask mask;

    private const string DisableMethodName = "Disable";
    
    [HideInInspector] public bool wasShotByPlayer;

    [Header("Effects")]
    public TrailRenderer trail;
    public ParticleSystem impactParticles;

    private void OnEnable()
    {
        //CancelInvoke(DISABLE_METHOD_NAME);
        Invoke(DisableMethodName, timeToDie);
    }

    private Vector3 _prevPos;

    private void Update()
    {
        MoveToPosition();
    }

    public void MoveToPosition()
    {
        _prevPos = transform.position;
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        Vector3 dir = (transform.position - _prevPos);
        
        RaycastHitGameobject(dir);
    }
    
    private void RaycastHitGameobject(Vector3 dir)
    {
        if(Physics.Raycast(transform.position, dir, out RaycastHit hit, dir.magnitude, mask))
        {
            Hit(hit);
        }
    }

    private void Hit(RaycastHit hit)
    {
        ImpactEffect(hit.point);
        Collision(hit.collider.gameObject);
        Disable();
    }
    
    private void ImpactEffect(Vector3 point)
    {
        if(impactParticles != null)
        {
            ParticleSystem effect = Instantiate(impactParticles);
            effect.transform.position = point;
            effect.transform.eulerAngles = transform.eulerAngles * -1;
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }

    public void Collision(GameObject other)
    {
        IDamageable obj = other.GetComponentInParent<IDamageable>() ?? other.GetComponentInChildren<IDamageable>();

        if (obj != null)
        {
            if (wasShotByPlayer)
            {
                Crosshair.instance.Hit();
            }

            obj.TakeDamage(damage);
        }
    }

    private void Disable()
    {
        CancelInvoke(DisableMethodName);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Vector3 dir = (transform.position - _prevPos);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + dir);
    }
}
