using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]
public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public EnemyLineOfSightChecker lineOfSightChecker;
    public NavMeshTriangulation triangulation;
    [SerializeField] private Animator _animator;
    public float updateRate = 0.1f;
    private NavMeshAgent _agent;
    private AgentLinkMover _linkMover;

    public EnemyState DefaultState;
    private EnemyState _state;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }

    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent OnStateChange;
    public float idleLocationRadius = 4f;
    public float idleMoveSpeedMultiplier = 0.5f;
    public Vector3[] waypoints = new Vector3[4];
    [SerializeField] private int WaypointIndex = 0;
    
    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private Coroutine _FollowCoroutine;

    private void Awake()
    {
        player = FindObjectOfType<Player>().transform;
        _agent = GetComponent<NavMeshAgent>();
        _linkMover = GetComponent<AgentLinkMover>();

        _linkMover.onLinkEnd += HandleLinkEnd;
        _linkMover.onLinkStart += HandleLinkStart;

        lineOfSightChecker.OnGainSight += HandleGainSight;
        lineOfSightChecker.OnLoseSight += HandleLoseSight;

        OnStateChange += HandleStateChange;
    }

    private void HandleGainSight(Player player)
    {
        _state = EnemyState.Chase;
    }

    private void HandleLoseSight(Player player)
    {
        _state = DefaultState;
    }

    private void OnDisable()
    {
        _state = DefaultState; // use _state to avoid triggering OnStateChange when recycling object in the pool
    }

    public void Spawn()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(triangulation.vertices[Random.Range(0, triangulation.vertices.Length)], out hit,
                2f, _agent.areaMask))
            {
                waypoints[i] = hit.position;
            }
            else
            {
                Debug.LogError("Unable to find position for navmesh near Triangulation vertex!");
            }
        }
        
        OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
        
        if (_FollowCoroutine == null)
        {
            _FollowCoroutine = StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogWarning("Called StartChasing on Enemy that is already chasing! This is likely a bug in some calling class!");
        }
    }
    
    private void Update()
    {
        _animator.SetBool(IsWalking, _agent.velocity.magnitude > 0.01f);
    }

    private void HandleLinkStart()
    {
        _animator.SetTrigger(Jump);
    }

    private void HandleLinkEnd()
    {
        _animator.SetTrigger(Landed);
    }
    
    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (oldState != newState)
        {
            if (_FollowCoroutine != null)
            {
                StopCoroutine(_FollowCoroutine);
            }

            if (oldState == EnemyState.Idle)
            {
                _agent.speed /= idleMoveSpeedMultiplier;
            }

            switch (newState)
            {
                case EnemyState.Idle:
                    _FollowCoroutine = StartCoroutine(DoIdleMotion());
                    break;
                case EnemyState.Patrol:
                    _FollowCoroutine = StartCoroutine(DoPatrolMotion());
                    break;
                case EnemyState.Chase:
                    _FollowCoroutine = StartCoroutine(FollowTarget());
                    break;
            }
        }
    }

    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        _agent.speed *= idleMoveSpeedMultiplier;

        while (true)
        {
            if (!_agent.enabled || !_agent.isOnNavMesh)
            {
                yield return wait;
            }
            else if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitCircle * idleLocationRadius;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(_agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f,
                    _agent.areaMask))
                {
                    _agent.SetDestination(hit.position);
                }
            }

            yield return wait;
        }
    }

    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);
        
        yield return new WaitUntil(() => _agent.enabled && _agent.isOnNavMesh);
        _agent.SetDestination(waypoints[WaypointIndex]);

        while (true)
        {
            if (_agent.isOnNavMesh && _agent.enabled && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                WaypointIndex++;

                if (WaypointIndex >= waypoints.Length)
                {
                    WaypointIndex = 0;
                }

                _agent.SetDestination(waypoints[WaypointIndex]);
            }

            yield return wait;
        }
    }
    
    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        while (enabled)
        {
            _agent.SetDestination(player.transform.position -
                                  (player.transform.position - transform.position).normalized * 0.5f);
            
            yield return wait;
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.DrawWireSphere(waypoints[i], 0.25f);
            if (i + 1 < waypoints.Length)
            {
                Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(waypoints[i], waypoints[0]);
            }
        }
    }
}
