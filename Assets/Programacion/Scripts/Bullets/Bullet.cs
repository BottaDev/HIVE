using System;
using Kam.Utils;
using UnityEngine;

public sealed class Bullet : PoolableObject
{
    [Header("Properties")]
    public float speed = 10f;
    public int damage = 1;
    public float timeToDie = 3f;
    public LayerMask mask;

    private const string DisableMethodName = "Disable";
    
    [HideInInspector] public bool wasShotByPlayer;

    [Header("Effects")]
    public TrailRenderer trail;
    public GameObject impactParticles;

    private bool firstFrame = true;

    private void OnEnable()
    {
        firstFrame = true;
        Invoke(DisableMethodName, timeToDie);
    }

    private Vector3 _prevPos;

    private void Update()
    {
        if (UIPauseMenu.paused)
        {
            return;
        }
        
        if (firstFrame)
        {
            firstFrame = false;
            //There is a specific bug that makes it so the first frame actually isn't checked for collision
            //This is due to "_prevPos" being at 0, 0, 0 on the first frame of the bullet

            _prevPos = transform.position - (transform.forward * Time.deltaTime * speed);
        }
        
        MoveToPosition();
    }

    public void MoveToPosition()
    {   
        Vector3 dir = (transform.position - _prevPos);
        RaycastHitGameobject(dir);
        
        _prevPos = transform.position;
        transform.position += transform.forward * Time.deltaTime * speed;
    }
    
    private void RaycastHitGameobject(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dir.magnitude, mask))
        {
            Hit(hit.collider);
        }
    }

    private void Hit(Collider hit)
    {
        ImpactEffect(transform.position);
        Collision(hit.gameObject, transform.position);
        Disable();
    }
    
    private void ImpactEffect(Vector3 point)
    {
        if(impactParticles != null)
        {
            GameObject obj = Instantiate(impactParticles);
            float maxTime = 0;
            foreach (Transform child in obj.transform)
            {
                ParticleSystem effect = child.GetComponent<ParticleSystem>();

                if (effect != null)
                {
                    effect.transform.position = point;
                    effect.transform.eulerAngles = transform.eulerAngles * -1;
                    effect.Play();
                    Destroy(effect.gameObject, effect.main.duration);

                    if (effect.main.duration > maxTime)
                    {
                        maxTime = effect.main.duration;
                    }
                }
            }
            
            Destroy(obj, maxTime);
        }
    }

    public void Collision(GameObject other, Vector3 point)
    {
        IDamageable obj = other.GetComponentInParent<IDamageable>() ?? other.GetComponentInChildren<IDamageable>();
        
        if (obj != null)
        {
            if (wasShotByPlayer)
            {
                UIGunSight.instance.Hit();
            }
            
            Popup.Create(point, damage.ToString(),KamColor.purple);

            obj.TakeDamage(damage);
        }
        
        IHittable hitobj = other.GetComponentInParent<IHittable>() ?? other.GetComponentInChildren<IHittable>();
        
        if (hitobj != null)
        {
            hitobj.Hit(transform);
        }
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        CancelInvoke(DisableMethodName);
    }

    private void OnDrawGizmos()
    {
        Vector3 dir = (transform.position - _prevPos);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + dir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (firstFrame) return;
        
        if (mask.CheckLayer(other.gameObject.layer))
        {
            Hit(other);
        }
    }

    
}
