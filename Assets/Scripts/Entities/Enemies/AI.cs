using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent), typeof(FOV))]
[DefaultExecutionOrder(1)]
public abstract class AI : Entity
{
    [System.Serializable]
    struct EXPAmounts
    {
        [Tooltip("This is the name that appears in editor for the entry.")]
        public string name;
        [Tooltip("This is the type of EXP to give in case of being of this type")]
        public PlayerLevel.ExpType type;
        [Tooltip("This is the actual amount of EXP to give in case of being of this type.")]
        public int expAmount;
    }

    [Header("EXP Parameters")]
    [SerializeField] List<EXPAmounts> ExpParameters;
    PlayerLevel.ExpType type;

    [Header("Death Parameters")]
    public bool useRigidBodyDeath = false;
    public List<Collider> deactivateColliders;
    public GameObject normalModel;
    public GameObject deathModel;
    public bool destroyGarbageAfterDelay = false;
    public float deathDelay = 5f;
    protected bool dying;

    [Header("Animator")]
    public bool useMovingAnim;
    public Animator anim;
    public string movingBool;
    
    
    [Header("AI Parameters")]
    [Range(0f, 3f)] public float attackRate = 1f;
    public float detectionRange = 25f;
    public float rotationSpeed = 5f;

    protected float _currentAttackRate;           
    protected Player _player;
    public NavMeshAgent _agent;
    protected FOV _fov;
    public bool _playerDetected;

    private Coroutine _followCoroutine;
    [SerializeField] private float updateRate = 0.1f;
    
    public Vector3[] Waypoints = new Vector3[4];
    [SerializeField]
    private int WaypointIndex = 0;
    
    public NavMeshTriangulation Triangulation = new NavMeshTriangulation();

    public Transform Player;

    [SerializeField] private ProgressBar HealthBar;

    public Utilities_ProgressBar healthSlider;
    public Utilities_CanvasGroupReveal reveal;

    protected override void Awake()
    {
        base.Awake();
        _fov = GetComponent<FOV>();
        _agent = GetComponent<NavMeshAgent>();
        _player = FindObjectOfType<Player>();
        //Prefab = this;

        _currentAttackRate = 0;
    
        healthSlider.SetRange(0,maxHealth);
        healthSlider.SetValue(maxHealth);

        //SetupHealthBar(HealthBarCanvas, Camera);
    }
    private void Start()
    {
        EventManager.Instance.Subscribe("OnPlayerEnteredUpgradeRoom", PauseIA);
        EventManager.Instance.Subscribe("OnPlayerLeftUpgradeRoom", ContinueIA);
        EventManager.Instance.Subscribe("GamePause", PauseIA);
        EventManager.Instance.Subscribe("GameUnPause", ContinueIA);
        
        if(deathModel != null)
        {
            deathModel.SetActive(false);
        }

        if (!_playerDetected && !_agent.isStopped)
        {
            DetectPlayer();
        }

        //Just get a random type for yourself
        switch (UnityEngine.Random.Range(0,3))
        {
            case 0:
                type = PlayerLevel.ExpType.Attack;
                break;
            case 1:
                type = PlayerLevel.ExpType.Defense;
                break;
            case 2:
                type = PlayerLevel.ExpType.Mobility;
                break;
        }
    }

    public void MoveTo(Vector3 position)
    {
        _agent.SetDestination(position);
    }
    protected virtual void Update()
    {
        if (dying) return;
        if (useMovingAnim)
        {
            anim.SetBool(movingBool, _agent.remainingDistance > _agent.stoppingDistance && !_agent.isStopped);
        }

        // Update the agent speed all the time...
        //_agent.speed = CurrentSpeed;
        
    }

    //private void CheckPlayerDistance()
    //{
    //    if (!_playerDetected)
    //    {
    //        float distance = Vector3.Distance(transform.position, _player.transform.position);
    //        if (distance <= detectionRange)
    //            StartCoroutine("ArtificialUpdate");   
    //    }
    //}

    public void PauseIA(params object[] p)
    {
        StopIA();
    }

    public void ContinueIA(params object[] p)
    {
        ResumeIA();
    }

    public void StopIA()
    {
        _agent.isStopped = true;
        _agent.speed = 0;
        _playerDetected = false;
    }

    public void ResumeIA()
    {
        _agent.isStopped = false;
        _agent.speed = CurrentSpeed;
        _playerDetected = true;
    }

    private void DetectPlayer()
    {
        _playerDetected = true;

        // Warn other enemies
        //IA2-P1
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
    
    public override void TakeDamage(int damage)
    {
        if (dying) return;
        
        if (!_playerDetected)
            DetectPlayer();
        
        CurrentHealth -= damage;

        if (healthSlider != null)
        {
            reveal.RevealTemporary();
            healthSlider.SetValue(CurrentHealth);
        }

        //HealthBar.SetProgress(CurrentHealth / maxHealth, 3);

        if(CurrentHealth <= 0)
        {
            KillAI();
        }
            
    }

    private void KillAI()
    {
        EventManager.Instance.Trigger("OnEnemyDeath");
        dying = true;
        _player.AddExp(type, ExpParameters.Where(x => x.type == type).First().expAmount);
        if (useRigidBodyDeath)
        {
            _agent.enabled = false;
            deathModel.transform.parent = null;
            deathModel.SetActive(true);

            foreach (var collider in deactivateColliders)
            {
                collider.enabled = false;
            }

            Destroy(normalModel.gameObject);
            if (destroyGarbageAfterDelay)
            {
                Invoke(nameof(DestroyDeathModel), deathDelay);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        if (HealthBar != null)
        {
            Destroy(HealthBar.gameObject);
        }
        
        
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EnemyDeath));
    }

    private void DestroyDeathModel()
    {
        Destroy(gameObject);
        Destroy(deathModel);
    }

    public void SetupHealthBar(Canvas canvas, Camera camera)
    {
        HealthBar.transform.SetParent(canvas.transform);
        if (HealthBar.TryGetComponent<FaceCamera>(out FaceCamera faceCamera))
        {
            faceCamera.Camera = camera;
        }
    }
    
    public void Spawn()
    {
        if (Triangulation.vertices != null)
        {
            for (int i = 0; i < Waypoints.Length; i++)
            {
                NavMeshHit Hit;
                if (NavMesh.SamplePosition(Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)], out Hit, 2f, _agent.areaMask))
                {
                    Waypoints[i] = Hit.position;
                }
                else
                {
                    Debug.LogError("Unable to find position for navmesh near Triangulation vertex!");
                }
            }
        }
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
