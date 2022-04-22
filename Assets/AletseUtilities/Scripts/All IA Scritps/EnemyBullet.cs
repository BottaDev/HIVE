using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : PoolableObject
{
    public float AutoDestroyTime = 5f;
    public float MoveSpeed = 2f;
    public float Damage = 5;
    public Rigidbody Rigidbody;
    private Transform target;

    private const string DISABLE_METHOD_NAME = "Disable";
    

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void OnEnable()
    {
        CancelInvoke(DISABLE_METHOD_NAME);
        Invoke(DISABLE_METHOD_NAME, AutoDestroyTime);
    }

    public virtual void Spawn(Vector3 Forward, float Damage, Transform Target)
    {
        this.Damage = Damage;
        target = Target;
        Rigidbody.AddForce(Forward * MoveSpeed, ForceMode.VelocityChange);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable damageable;

        if (other.TryGetComponent<IDamageable>(out damageable))
        {
            damageable.TakeDamage(Damage);
        }

        Disable();
    }

    protected void Disable()
    {
        CancelInvoke(DISABLE_METHOD_NAME);
        Rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
