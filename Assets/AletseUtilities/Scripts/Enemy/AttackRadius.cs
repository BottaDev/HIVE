using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackRadius : MonoBehaviour
{
    public SphereCollider collider;
    private List<IDamagable> _damagables = new List<IDamagable>();
    public int damage = 10;
    public float attackDelay = 0.5f;

    public delegate void AttackEvent(IDamagable target);

    public AttackEvent onAttack;
    private Coroutine attackCoroutine;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();

        if (damagable != null)
        {
            _damagables.Add(damagable);

            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(Attack());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamagable damageable = other.GetComponent<IDamagable>();

        if (damageable != null)
        {
            _damagables.Remove(damageable);
            if (_damagables.Count == 0)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(attackDelay);

        yield return wait;

        IDamagable closestDamageable = null;
        float closestDistance = float.MaxValue;

        while (_damagables.Count > 0)
        {
            for (int i = 0; i < _damagables.Count; i++)
            {
                Transform damageableTransform = _damagables[i].GetTransform();
                float distance = Vector3.Distance(transform.position, damageableTransform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDamageable = _damagables[i];
                }
            }

            if (closestDamageable != null)
            {
                onAttack?.Invoke(closestDamageable);
                closestDamageable.TakeDamage(damage);
            }

            closestDamageable = null;
            closestDistance = float.MaxValue;

            yield return wait;

            _damagables.RemoveAll(DisabledDamageables);
        }

        attackCoroutine = null;
    }

    private bool DisabledDamageables(IDamagable damagable)
    {
        return _damagables != null && !damagable.GetTransform().gameObject.activeSelf;
    }
}
