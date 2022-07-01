using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using IA2;
public class Emilrang : Entity
{
    private Rigidbody rb;
    public Player player;
    public float speed;
    public float speedModifier = 3;
    public float maxDistanceAbovePlayer;
    public float maxDistanceBelowPlayer;
    public float recoverDistance;
    public float attackEvery;
    public int projectileAmount;
    public EmilrangBodyPart[] BodyParts;
    public Transform middlePoint;
    public Utilities_ImageLazyProgressBar hpBar;
    public UnityEvent onDeath;
    private EventFSM<MovementState> _movementFsm;
    private EventFSM<PhaseState> _phaseFsm;
    
    protected override void Awake()
    {
        base.Awake();
        SetupMovementFSM();
        SetupPhaseFSM();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hpBar.SetRange(0, maxHealth);
        hpBar.SetValueInstant(CurrentHealth);
        EventManager.Instance.Subscribe("GamePause", Pause);
        EventManager.Instance.Unsubscribe("GameUnPause", Unpause);
    }

    public enum MovementState{Idle, Speedup, Slowdown, Death}
    public enum PhaseState{One, Two, Death}
    void SetupMovementFSM()
    {
        #region Declare
        
        var idle = new State<MovementState>("Idle");
        var speedup = new State<MovementState>("Speedup");
        var slowdown = new State<MovementState>("Slowdown");
        var death = new State<MovementState>("Death");

        #endregion

        #region MakeTransitions

        StateConfigurer.Create(idle)
            .SetTransition(MovementState.Speedup, speedup)
            .SetTransition(MovementState.Slowdown, slowdown)
            .SetTransition(MovementState.Death, death)
            .Done();
        
        StateConfigurer.Create(speedup)
            .SetTransition(MovementState.Idle, idle)
            .SetTransition(MovementState.Slowdown, slowdown)
            .SetTransition(MovementState.Death, death)
            .Done();
        
        StateConfigurer.Create(slowdown)
            .SetTransition(MovementState.Idle, idle)
            .SetTransition(MovementState.Speedup, speedup)
            .SetTransition(MovementState.Death, death)
            .Done();
        
        StateConfigurer.Create(death)
            .SetTransition(MovementState.Idle, idle)
            .Done();
        
        #endregion

        #region StateBehaviour

        idle.OnEnter += x =>
        {
            Debug.Log("Emilrang state: idle");
        };
        
        idle.OnUpdate += () =>
        {
            MoveUp(speed);

            float distance = player.transform.position.y - middlePoint.position.y ;
            if (distance < 0)
            {
                //player is below
                if (Mathf.Abs(distance) > maxDistanceAbovePlayer)
                {
                    _movementFsm.SendInput(MovementState.Slowdown);
                }
            }
            else
            {
                //player is above
                if (Mathf.Abs(distance) > maxDistanceBelowPlayer)
                {
                    _movementFsm.SendInput(MovementState.Speedup);
                }
            }
        };
        
        slowdown.OnEnter += x =>
        {
            Debug.Log("Emilrang state: speedup");
        };
        speedup.OnUpdate += () =>
        {
            MoveUp(speed * speedModifier);
            
            float distance = player.transform.position.y - middlePoint.position.y ;
            if (Mathf.Abs(distance) < recoverDistance)
            {
                _movementFsm.SendInput(MovementState.Idle);
            }
        };

        slowdown.OnEnter += x =>
        {
            Debug.Log("Emilrang state: slowdown");
        };
        slowdown.OnUpdate += () =>
        {
            MoveUp(speed / speedModifier);
            
            float distance = player.transform.position.y - middlePoint.position.y ;
            if (Mathf.Abs(distance) < recoverDistance)
            {
                _movementFsm.SendInput(MovementState.Idle);
            }
        };
        
        death.OnEnter += x =>
        {
            Debug.Log("Emilrang state: death");
            onDeath.Invoke();
            AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.Explosion));
            AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.Explosion));
            _movementFsm.SendInput(MovementState.Idle);
        };


        #endregion
        
        _movementFsm = new EventFSM<MovementState>(idle);
    }
    void SetupPhaseFSM()
    {
        #region Declare
        
        var one = new State<PhaseState>("PhaseOne");
        var two = new State<PhaseState>("PhaseTwo");
        var death = new State<PhaseState>("Death");

        #endregion

        #region MakeTransitions

        StateConfigurer.Create(one)
            .SetTransition(PhaseState.Two, two)
            .SetTransition(PhaseState.Death, death)
            .Done();
        
        StateConfigurer.Create(two)
            .SetTransition(PhaseState.Death, death)
            .Done();

        StateConfigurer.Create(death)
            .Done();
        
        #endregion

        #region StateBehaviour

        one.OnEnter += x =>
        {
            Debug.Log("Emilrang: Phase One");
            
            StartCoroutine(Delay(delegate
            {
                foreach (var part in BodyParts)
                {
                    part.SendInput(EmilrangBodyPart.Attack.Shoot);
                }
            }, 0));
        };

        one.OnUpdate += () =>
        {
            if (((CurrentHealth * 100) / maxHealth) < 50)
            {
                _phaseFsm.SendInput(PhaseState.Two);
            }
        };

        two.OnEnter += x =>
        {
            Debug.LogWarning("Emilrang: Phase Two");
            foreach (var part in BodyParts.Where(x => x.changeOnPhaseTwo))
            {
                part.SendInput(EmilrangBodyPart.Attack.Lazer);
            }
        };
        
        death.OnEnter += x =>
        {
            foreach (var part in BodyParts)
            {
                part.SendInput(EmilrangBodyPart.Attack.Idle);
            }
        };
        #endregion
        
        _phaseFsm = new EventFSM<PhaseState>(one);
    }
    // Update is called once per frame
    void Update()
    {
        if (UIPauseMenu.paused) return;
        _movementFsm.Update();
        _phaseFsm.Update();
    }

    public void MoveUp(float speed)
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    public override void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        hpBar.SetValue(CurrentHealth);
        if(CurrentHealth <= 0)
        {
            _movementFsm.SendInput(MovementState.Death);
            _phaseFsm.SendInput(PhaseState.Death);
            StopAllCoroutines();
        }
    }

    void Pause(params object[] obj)
    {
        foreach (var part in BodyParts)
        {
            part.Pause();
        }
    }

    void Unpause(params object[] obj)
    {
        foreach (var part in BodyParts)
        {
            part.Unpause();
        }
    }

    IEnumerator Delay(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        
        action.Invoke();
    }
}
