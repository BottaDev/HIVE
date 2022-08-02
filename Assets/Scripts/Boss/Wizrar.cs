using System.Collections;
using System.Collections.Generic;
using IA2;
using UnityEngine;
using UnityEngine.Events;

public class Wizrar : Entity
{
    private Rigidbody rb;
    public float shootTime;
    public float spawnTime;
    public Transform idleLookAtPoint;
    public Animator anim;
    public Utilities_ConstantTriggerEvent triggerShoot;
    public Utilities_ConstantTriggerEvent[] triggerSpawns;
    public Utilities_SinewaveMovement sine;
    public Utilities_SliderLinearProgressBar hpBar;
    public Utilities_CanvasGroupReveal reveal;
    public GameObject normalModel;
    public GameObject deathModel;
    public ParticleSystem deathParticles;
    public UnityEvent onShoot;
    public UnityEvent onSpawn;
    public UnityEvent onDeath;
    public Attack currentAttack;
    private EventFSM<Attack> _attackFSM;
    private Player _player;
    private static readonly int Paused = Animator.StringToHash("Paused");

    protected override void Awake()
    {
        base.Awake();
        SetupAttackFSM();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hpBar.SetRange(0, maxHealth);
        hpBar.SetValueInstant(CurrentHealth);
        EventManager.Instance.Subscribe("GamePause", Pause);
        EventManager.Instance.Unsubscribe("GameUnPause", Unpause);
        EventManager.Instance.Subscribe("SendPlayerReference", GetPlayerReference);
        EventManager.Instance.Trigger("NeedsPlayerReference");
    }
    
    private void GetPlayerReference(params object[] p)
    {
        _player = (Player)p[0];
    }

    public enum Attack{Frozen, Idle, Shoot, Spawn, Death}
    void SetupAttackFSM()
    {
        #region Declare
        
        var frozen = new State<Attack>("Frozen");
        var idle = new State<Attack>("Idle");
        var shoot = new State<Attack>("Shoot");
        var spawn = new State<Attack>("Spawn");
        var death = new State<Attack>("Death");

        #endregion

        #region MakeTransitions

        StateConfigurer.Create(frozen)
            .SetTransition(Attack.Idle, idle)
            .SetTransition(Attack.Shoot, shoot)
            .SetTransition(Attack.Spawn, spawn)
            .SetTransition(Attack.Death, death)
            .Done();
        
        StateConfigurer.Create(idle)
            .SetTransition(Attack.Frozen, frozen)
            .SetTransition(Attack.Shoot, shoot)
            .SetTransition(Attack.Spawn, spawn)
            .SetTransition(Attack.Death, death)
            .Done();
        
        StateConfigurer.Create(shoot)
            .SetTransition(Attack.Frozen, frozen)
            .SetTransition(Attack.Idle, idle)
            .SetTransition(Attack.Spawn, spawn)
            .SetTransition(Attack.Death, death)
            .Done();
        
        StateConfigurer.Create(spawn)
            .SetTransition(Attack.Frozen, frozen)
            .SetTransition(Attack.Idle, idle)
            .SetTransition(Attack.Shoot, shoot)
            .SetTransition(Attack.Death, death)
            .Done();
        
        StateConfigurer.Create(death)
            .SetTransition(Attack.Frozen, frozen)
            .Done();
        
        #endregion

        #region StateBehaviour

        frozen.OnEnter += x =>
        {
            triggerShoot.StopTriggering();
            foreach (var trigger in triggerSpawns)
            {
                trigger.StopTriggering();
            }
            sine.Stop();
            anim.SetBool(Paused, true);
        };
        
        idle.OnEnter += x =>
        {
            Debug.Log("Wizrar Attack: idle");
            AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.EggSpawningEffect));
            Instantiate(deathParticles, transform.position, Quaternion.identity, transform.parent);
            triggerShoot.StopTriggering();
            foreach (var trigger in triggerSpawns)
            {
                trigger.StopTriggering();
            }
            
            sine.Resume();
            anim.SetBool(Paused, false);
            _attackFSM.SendInput(Attack.Shoot);
        };
        
        idle.OnUpdate += () =>
        {
            LookAt(idleLookAtPoint);
        };
        
        shoot.OnEnter += x =>
        {
            Debug.Log("Wizrar Attack: shoot");
            triggerShoot.StartTriggering();
            foreach (var trigger in triggerSpawns)
            {
                trigger.StopTriggering();
            }

            StartCoroutine(Delay(delegate { _attackFSM.SendInput(Attack.Spawn); }, shootTime));
        };
        
        shoot.OnUpdate += () =>
        {
            triggerShoot.transform.LookAt(_player.transform);
            LookAt(_player.transform);
        };
        
        spawn.OnEnter += x =>
        {
            Debug.Log("Wizrar Attack: spawn");
            triggerShoot.StopTriggering();
            foreach (var trigger in triggerSpawns)
            {
                trigger.StartTriggering();
            }

            StartCoroutine(Delay(delegate { _attackFSM.SendInput(Attack.Shoot); }, spawnTime));
        };
        
        spawn.OnUpdate += () =>
        {
            
        };

        death.OnEnter += x =>
        {
            Debug.Log("Wizrar Death");
            Death();
        };

        #endregion
        
        _attackFSM = new EventFSM<Attack>(frozen);
    }
    // Update is called once per frame
    void Update()
    {
        if (UIPauseMenu.paused) return;
        _attackFSM.Update();
    }

    public void Death()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity, transform.parent);
        normalModel.SetActive(false);
        deathModel.transform.parent = transform.parent;
        deathModel.SetActive(true);
        onDeath.Invoke();
        Destroy(gameObject);
    }

    public void LookAt(Transform lookat)
    {
        Vector3 rot = transform.rotation.eulerAngles;
        transform.LookAt(lookat);
        transform.eulerAngles = new Vector3(rot.x, transform.rotation.eulerAngles.y, rot.z);
    }

    public override void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        hpBar.SetValue(CurrentHealth);
        reveal.RevealTemporary();
        if(CurrentHealth <= 0)
        {
            _attackFSM.SendInput(Attack.Death);
            StopAllCoroutines();
        }
    }

    public void StartBoss()
    {
        _attackFSM.SendInput(Attack.Idle);
    }
    
    void Pause(params object[] obj)
    {
        switch (currentAttack)
        {
            case Attack.Shoot:
                triggerShoot.StopTriggering();
                break;
        }
        
    }

    void Unpause(params object[] obj)
    {
        switch (currentAttack)
        {
            case Attack.Shoot:
                triggerShoot.StartTriggering();
                break;
        }
    }
    
    IEnumerator Delay(System.Action action, float time)
    {
        yield return new WaitForSeconds(time);
        
        action.Invoke();
    }
}
