using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(FOV))]
public abstract class AI : Entity
{
    [System.Serializable]
    struct EXPAmounts
    {
        [Tooltip("This is the name that appears in editor for the entry.")]
        public string name;
        [Tooltip("This is the type of EXP to give in case of being of this type")]
        public Player.EXPType type;
        [Tooltip("This is the actual amount of EXP to give in case of being of this type.")]
        public int expAmount;
    }

    [Header("EXP Parameters")]
    [SerializeField] List<EXPAmounts> ExpParameters;
    Player.EXPType type;

    [Header("AI Parameters")]
    [Range(0f, 3f)] public float attackRate = 1f;
    public float detectionRange = 25f;
    public float rotationSpeed = 5f;
    public bool isAEnemySpawner = false;

    
    protected float _currentAttackRate;           
    protected Player _player;
    public NavMeshAgent _agent;
    protected FOV _fov;
    public bool _playerDetected;

    private Coroutine _followCoroutine;
    [SerializeField] private float updateRate = 0.1f;
    
    
    protected override void Awake()
    {
        base.Awake();
        _fov = GetComponent<FOV>();
        _agent = GetComponent<NavMeshAgent>();
        _player = FindObjectOfType<Player>();
        
        _currentAttackRate = 0;
    }
    private void Start()
    {
        //Just get a random type for yourself
        switch (UnityEngine.Random.Range(0,3))
        {
            case 0:
                type = Player.EXPType.Attack;
                break;
            case 1:
                type = Player.EXPType.Defense;
                break;
            case 2:
                type = Player.EXPType.Mobility;
                break;
        }
    }

    protected virtual void Update()
    {
        CheckPlayerDistance();

        // Update the agent speed all the time...
        _agent.speed = CurrentSpeed;
    }

    private void CheckPlayerDistance()
    {
        if (!_playerDetected)
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance <= detectionRange)
                DetectPlayer();   
        }
    }

    public void StartChasing()
    {
        if (_followCoroutine == null)
        {
            _followCoroutine = StartCoroutine(FollowTarget());
        }
        else
        {
            Debug.LogWarning("Called StartChasing on Enemy that is already chasing! This is likely a bug in some calling");
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        while (enabled)
        {
            _agent.SetDestination(_player.transform.position);
            yield return wait;
        }
    }

    private void DetectPlayer()
    {
        _playerDetected = true;

        // Warn other enemies
        List<AI> nearbyEnemies = FindObjectsOfType<AI>()
            .Where(x => (transform.position - x.transform.position).magnitude <= detectionRange)
            .ToList();
        
        foreach (AI ai in nearbyEnemies)
        {
            ai._playerDetected = true;
        }
    }

    protected void MoveToPosition(Vector3 position)
    {
        _agent.destination = position;
    }
    
    protected void RotateTowards (Vector3 target) 
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    protected virtual void Attack() { }
    
    public override void TakeDamage(float damage)
    {
        if (!_playerDetected)
            DetectPlayer();
        
        CurrentHealth -= damage;
        
        if(CurrentHealth <= 0)
        {
            _player.AddEXP(type, ExpParameters.Where(x => x.type == type).First().expAmount);
            KillAI();
        }
            
    }

    private void KillAI()
    {
        EventManager.Instance.Trigger("OnEnemyDeath");

        Destroy(gameObject);
    }

    //public override void OnDisable()
    //{
    //    base.OnDisable();
    //
    //    _agent.enabled = false;
    //}

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
