using System;
using System.Collections;
using System.Collections.Generic;
using IA2;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EmilrangBodyPart : MonoBehaviour
{
    public Attack currentAttack;
    public Transform[] attackPoints;
    public Utilities_ConstantTriggerEvent[] ConstantTriggers;
    public Utilities_ConstantlyRotate rotate;
    public Utilities_SinewaveMovement sine;
    public EmilrangLazerAttack[] lazerAttacks;
    public Electric lazerEffectPrefab;
    public LayerMask lazerMask;
    public bool changeOnPhaseTwo;

    private Dictionary<Transform, Tuple<Electric, Transform>> lazerEffects = new Dictionary<Transform, Tuple<Electric, Transform>>();
    public EventFSM<Attack> attackFSM;
    public enum Attack
    {
        Shoot, Lazer, Idle
    }

    private void Awake()
    {
        SetupPhaseFSM();
    }

    void SetupPhaseFSM()
    {
        #region Declare
        
        var shoot = new State<Attack>("Shoot");
        var lazer = new State<Attack>("lazer");
        var idle = new State<Attack>("Idle");

        #endregion

        #region MakeTransitions

        StateConfigurer.Create(shoot)
            .SetTransition(Attack.Lazer, lazer)
            .SetTransition(Attack.Idle, idle)
            .Done();
        
        StateConfigurer.Create(lazer)
            .SetTransition(Attack.Shoot, shoot)
            .SetTransition(Attack.Idle, idle)
            .Done();

        StateConfigurer.Create(idle)
            .SetTransition(Attack.Shoot, shoot)
            .SetTransition(Attack.Lazer, lazer)
            .Done();
        
        #endregion

        #region StateBehaviour
        idle.OnEnter += x =>
        {
            StopLazer();
            StopShoot();
        };
        
        shoot.OnEnter += x =>
        {
            StopLazer();
            StartShoot();
        };

        lazer.OnEnter += x =>
        {
            StopShoot();
            StartLazer();
        };

        lazer.OnUpdate += () =>
        {
            foreach (var point in attackPoints)
            {
                if (Physics.Raycast(point.position, point.TransformDirection(Vector3.forward), out RaycastHit hit, 999f, lazerMask))
                {
                    lazerEffects[point].Item2.position = hit.point;
                }
            }
        };
        #endregion
        
        attackFSM = new EventFSM<Attack>(idle);
    }

    private void Update()
    {
        attackFSM.Update();
    }

    void StartLazer()
    {
        currentAttack = Attack.Lazer;
        
        foreach (var point in attackPoints)
        {
            Electric effect = Instantiate(lazerEffectPrefab);
            effect.transformPointA = point;
            effect.transformPointB = new GameObject().transform;
            lazerEffects.Add(point, Tuple.Create(effect,effect.transformPointB));
        }
        
        foreach (var attack in lazerAttacks)
        {
            attack.activated = true;
        }
    }

    void StopLazer()
    {
        foreach (var attack in lazerAttacks)
        {
            attack.activated = false;
        }
        
        foreach (var point in attackPoints)
        {
            if (lazerEffects.ContainsKey(point))
            {
                Destroy(lazerEffects[point].Item1.gameObject);
                Destroy(lazerEffects[point].Item2.gameObject);
                lazerEffects.Remove(point);
            }
        }
    }
    
    void StartShoot()
    {
        currentAttack = Attack.Shoot;
        foreach (var point in ConstantTriggers)
        {
            point.StartTriggering();
        }
    }
    void StopShoot()
    {
        foreach (var point in ConstantTriggers)
        {
            point.StopTriggering();
        }
    }

    public void Unpause()
    {
        StartShoot();
        rotate.Resume();
        sine.Resume();
    }
    
    public void Pause()
    {
        StopShoot();
        rotate.Stop();
        sine.Stop();
    }

    public void SendInput(Attack attack)
    {
        attackFSM.SendInput(attack);
    }
}
