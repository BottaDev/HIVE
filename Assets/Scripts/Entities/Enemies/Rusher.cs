using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Rusher : AI
{
    [Header("Rusher Parameters")] 
    [Tooltip("The normal distance between the start position and the final position")] public float attackJumpLength = 5f; 
    [Tooltip("The duration of the attack (Should be the same as the animation)")] public float attackDuration = 0.3f;
    public float attackDistance = 7f;
    [Tooltip("The time it takes to do the attack")] public float waitTime = 0.3f;
    [Tooltip("The Rusher collider")] public Collider collider;
    
    public Animator _animator;
    private bool _isJumpping;
    private bool _isAttacking;
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
            // Player is in range, and is being seen
            if (_fov.ApplyFOV(_player.transform.position))   
            {
                if (_currentAttackRate <= 0 && !_isAttacking)
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
    
    protected override void Attack()
    {
        StartCoroutine(AttackMelee());
    }

    private IEnumerator AttackMelee()
    {
        //float animDuration = _animator.GetCurrentAnimatorStateInfo(0).length;
        _agent.isStopped = true;
        _isJumpping = true;
        _isAttacking = true;
        yield return new WaitForSeconds(waitTime);
        
        collider.enabled = false;
        _animator.SetBool(IsAttacking, true);

        StartCoroutine(LerpPosition(GetEndPosition(), attackDuration));

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
        dirToTarget.y = transform.position.y;
        
        if (Physics.Raycast(transform.position, dirToTarget, out var hit, attackJumpLength, _fov.obstacleMask))
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
            time += Time.deltaTime; ;
            yield return null;
        }
        
        transform.position = targetPosition;
        _isAttacking = false;
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
