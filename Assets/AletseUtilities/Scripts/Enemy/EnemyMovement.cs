using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]
public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    [SerializeField] private Animator _animator;
    public float updateRate = 0.1f;
    private NavMeshAgent _agent;
    private AgentLinkMover _linkMover;
    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private Coroutine _followCoroutine;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _linkMover = GetComponent<AgentLinkMover>();

        _linkMover.onLinkEnd += HandleLinkEnd;
        _linkMover.onLinkStart += HandleLinkStart;
    }

    public void StartChasing()
    {
        if (_followCoroutine == null)
        {
            _followCoroutine = StartCoroutine(FollowTarget());
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
}
