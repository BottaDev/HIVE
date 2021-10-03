using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Rusher : AI
{
    [Header("Rusher Parameters")] 
    [Tooltip("The normal distance between the start position and the final position")] public float attackJumpLength = 5f; 
    [Tooltip("The duration of the attack (Should be the same as the animation)")] public float attackTime = 0.3f;
    public float attackDistance = 7f;
    [Tooltip("The Rusher collider")] public Collider collider;
    
    public Animator _animator;
    private bool _isJumpping;
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    private float damage = 1;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (_playerDetected)
            ChasePlayer();
    }

    private void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= attackDistance)
        {
            if (ApplyFOV(_player.transform.position))   // Player is in range, and is being seen
            {
                if (_currentAttackRate <= 0)
                {
                    _isJumpping = false;
                    Attack();
                }
            }
            else
            {
                RotateTowards(_player.transform.position);
            }
        }
        else
        {
            MoveToPosition(_player.transform.position);
        }
        
        _currentAttackRate -= Time.deltaTime;
    }

    // ToDo Botta: Fix animation bool
    protected override void Attack()
    {
        float animDuration = _animator.GetCurrentAnimatorStateInfo(0).length;
        
        _animator.SetBool(IsAttacking, true);
        collider.enabled = false;
        _agent.isStopped = true;
        _isJumpping = true;

        StartCoroutine(LerpPosition(GetEndPosition(), attackTime));
        StartCoroutine(WaitTime(animDuration));
        
        
        _animator.SetBool(IsAttacking, false);
        _currentAttackRate = attackRate;
        collider.enabled = true;
        _agent.isStopped = false;
    }

    /// <summary>
    /// Returns the end position of the attack
    /// </summary>
    /// <returns></returns>
    private Vector3 GetEndPosition()
    {
        Vector3 endPosition;
        Vector3 dirToTarget = _player.transform.position - transform.position;
        
        if (Physics.Raycast(transform.position, dirToTarget, out var hit, attackJumpLength, obstacleMask))
        {
            // Raycast hit an obstacle...
            endPosition = hit.point;
        }
        else
        {
            // Raycast didn't hit an obstacle...
            endPosition = transform.position + dirToTarget.normalized * attackJumpLength;
        }

        return endPosition;
    }
    
    private IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    public override void TakeDamage(float damage)
    {
        if(CurrentHealth <= 0)
            Destroy(gameObject);

        CurrentHealth -= damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 && _isJumpping)
        {
            EventManager.Instance.Trigger("OnPlayerDamaged", damage);
            _isJumpping = false;
        }
    }
}
