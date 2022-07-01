using System;
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

    bool firstFrame = true;

    private void OnEnable()
    {
        firstFrame = true;
        Invoke(DisableMethodName, timeToDie);
    }

    private Vector3 _prevPos;

    private void Update()
    {
        /*
        if (firstFrame)
        {
            firstFrame = false;
            //There is a specific bug that makes it so the first frame actually isn't checked for collision
            //This is due to "_prevPos" being at 0, 0, 0 on the first frame of the bullet
            //To fix this, we move the bullet back, then save its position, and move it forward within one frame.

            transform.Translate(-translation);
            _prevPos = transform.position;
            transform.Translate(translation);
        }*/

        //Vector3 dir = (transform.position - _prevPos);
        //RaycastHitGameobject(dir);
    }

    private void FixedUpdate()
    {
        MoveToPosition();
    }

    public void MoveToPosition()
    {
        _prevPos = transform.position;
        transform.position += transform.forward * Time.deltaTime * speed;
    }
    
    private void RaycastHitGameobject(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dir.magnitude, mask))
        {
            //Hit(hit);
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

            foreach (Transform child in obj.transform)
            {
                ParticleSystem effect = child.GetComponent<ParticleSystem>();

                if (effect != null)
                {
                    effect.transform.position = point;
                    effect.transform.eulerAngles = transform.eulerAngles * -1;
                    effect.Play();
                    Destroy(effect.gameObject, effect.main.duration);
                }
            }
            
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
            
            Popup.Create(point, damage.ToString(),Color.red);

            obj.TakeDamage(damage);
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
        if (mask.CheckLayer(other.gameObject.layer))
        {
            Hit(other);
        }
    }
}
