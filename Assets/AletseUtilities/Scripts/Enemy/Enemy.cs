using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : PoolableObject, IDamagable
{
    public AttackRadius attackRadius;
    public Animator animator;
    public EnemyMovement movement;
    public NavMeshAgent agent;
    public EnemyScriptableObject enemyScriptableObject;

    private Coroutine LookCoroutine;
    public float health;

    private const string ATTACK_TRIGGER = "Attack";

    private void Awake()
    {
        attackRadius.onAttack += OnAttack;
    }

    private void OnAttack(IDamagable target)
    {
        animator.SetTrigger(ATTACK_TRIGGER);

        if (LookCoroutine != null)
        {
            StopCoroutine((LookCoroutine));
        }

        LookCoroutine = StartCoroutine(LookAt(target.GetTransform()));
    }

    private IEnumerator LookAt(Transform target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = lookRotation;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        agent.enabled = false;
    }

    public virtual void OnEnable()
    {
        SetupAgentFromConfiguration();
    }

    public virtual void SetupAgentFromConfiguration()
    {
        agent.acceleration = enemyScriptableObject.acceleration;
        agent.angularSpeed = enemyScriptableObject.angularSpeed;
        agent.areaMask = enemyScriptableObject.areaMask;
        agent.avoidancePriority = enemyScriptableObject.avoidancePriority;
        agent.baseOffset = enemyScriptableObject.baseOffset;
        agent.height = enemyScriptableObject.height;
        agent.obstacleAvoidanceType = enemyScriptableObject.obstacleAvoidanceType;
        agent.radius = enemyScriptableObject.radius;
        agent.speed = enemyScriptableObject.speed;
        agent.stoppingDistance = enemyScriptableObject.stoppingDistance;

        movement.updateRate = enemyScriptableObject.aIUpdateInterval;
        
        health = enemyScriptableObject.health;
        
        attackRadius.collider.radius = enemyScriptableObject.attackRadius;
        //attackRadius.attackDelay
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
