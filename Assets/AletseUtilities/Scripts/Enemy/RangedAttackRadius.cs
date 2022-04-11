using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedAttackRadius : AttackRadius
{
    public NavMeshAgent agent;
    public Bullet bulletPrefab;
    public Vector3 bulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask mask;
    private ObjectPool bulletPool;
    [SerializeField] private float SpherecastRadius = 0.1f;
    private RaycastHit hit;
    private IDamagable targetDamageable;
    private Bullet bullet;
    
    public void CreateBulletPool()
    {
        if (bulletPool == null)
        {
            bulletPool = ObjectPool.CreateInstance(bulletPrefab, Mathf.CeilToInt((1/attackDelay) * bulletPrefab.timeToDie));
        }
    }

    protected override IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(attackDelay);

        yield return wait;

        while (_damagables.Count > 0)
        {
            for (int i = 0; i < _damagables.Count; i++)
            {
                if (HasLineOfSightTo(_damagables[i].GetTransform()))
                {
                    targetDamageable = _damagables[i];
                    onAttack?.Invoke(_damagables[i]);
                    agent.enabled = false;
                    break;
                }
            }

            if (targetDamageable != null)
            {
                PoolableObject poolableObject = bulletPool.GetObject();
                if (poolableObject != null)
                {
                    bullet = poolableObject.GetComponent<Bullet>();

                    bullet.trail.Clear();
                    bullet.damage = damage;
                    var bulletTransform = bullet.transform;
                    bulletTransform.position = transform.position + bulletSpawnOffset;
                    bulletTransform.rotation = agent.transform.rotation;
                }
            }
            else
            {
                agent.enabled = true; // no target in line of sight, keep trying to get closer
            }

            yield return wait;

            if (targetDamageable == null || !HasLineOfSightTo(targetDamageable.GetTransform()))
            {
                agent.enabled = true;
            }

            _damagables.RemoveAll(DisabledDamageables);
        }

        agent.enabled = true;
        attackCoroutine = null;
    }

    private bool HasLineOfSightTo(Transform target)
    {
        if (Physics.SphereCast(transform.position + bulletSpawnOffset, SpherecastRadius,
            (target.position + bulletSpawnOffset) - (transform.position + bulletSpawnOffset).normalized, out hit,
            collider.radius, mask))
        {
            IDamagable damageable;
            if (hit.collider.TryGetComponent<IDamagable>(out damageable))
            {
                return damageable.GetTransform() == target;
            }
            
            return hit.collider.GetComponent<IDamagable>() != null;
        }

        return false;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (attackCoroutine == null)
        {
            agent.enabled = true;
        }
    }
}
