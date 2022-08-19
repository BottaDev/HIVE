using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using IA2;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EmilrangBodyOpening : MonoBehaviour
{
    public Attack currentAttack;
    [FormerlySerializedAs("constantTrigger")] public Utilities_ConstantTriggerEvent shootTriggerEvent;
    public Utilities_ConstantTriggerEvent enemySpawnTriggerEvent;
    public EmilrangLazerAttack lazerAttack;
    public Electric lazerEffectPrefab;
    public LayerMask lazerMask;

    private Tuple<Electric, Transform> lazerEffect;
    public EventFSM<Attack> attackFSM;

    public Attack phase2Attack;
    public Attack phase3Attack;

    public enum Attack
    {
        Shoot, Lazer, Idle, SpawnEnemy
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
        var spawnEnemy = new State<Attack>("SpawnEnemy");
        var idle = new State<Attack>("Idle");

        #endregion

        #region MakeTransitions

        StateConfigurer.Create(shoot)
            .SetTransition(Attack.Lazer, lazer)
            .SetTransition(Attack.SpawnEnemy, spawnEnemy)
            .SetTransition(Attack.Idle, idle)
            .Done();
        
        StateConfigurer.Create(lazer)
            .SetTransition(Attack.Shoot, shoot)
            .SetTransition(Attack.SpawnEnemy, spawnEnemy)
            .SetTransition(Attack.Idle, idle)
            .Done();

        StateConfigurer.Create(spawnEnemy)
            .SetTransition(Attack.Shoot, shoot)
            .SetTransition(Attack.Lazer, lazer)
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
            StopSpawnEnemy();
            StartShoot();
        };

        lazer.OnEnter += x =>
        {
            StopShoot();
            StopSpawnEnemy();
            StartLazer();
        };

        lazer.OnUpdate += () =>
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, 999f, lazerMask))
            {
                lazerEffect.Item2.position = hit.point;
            }
        };
        
        spawnEnemy.OnEnter += x =>
        {
            StopShoot();
            StopLazer();
            StartSpawnEnemy();
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
        if (!lazerAttack.activated)
        {
            currentAttack = Attack.Lazer;


            Electric effect = Instantiate(lazerEffectPrefab);
            effect.transformPointA = transform;
            effect.transformPointB = new GameObject().transform;
            lazerEffect = Tuple.Create(effect, effect.transformPointB);


            lazerAttack.SetActive(true);
        }
    }

    void StopLazer()
    {
        if (lazerAttack.activated)
        {
            lazerAttack.SetActive(false);
        
            Destroy(lazerEffect.Item1.gameObject);
            Destroy(lazerEffect.Item2.gameObject);
        }
    }
    
    void StartShoot()
    {
        currentAttack = Attack.Shoot;
        shootTriggerEvent.StartTriggering();
    }
    void StopShoot()
    {
        shootTriggerEvent.StopTriggering();
    }

    void StartSpawnEnemy()
    {
        currentAttack = Attack.SpawnEnemy;
        enemySpawnTriggerEvent?.StartTriggering();
    }

    void StopSpawnEnemy()
    {
        enemySpawnTriggerEvent?.StopTriggering();
    }

    public void Unpause()
    {
        switch (currentAttack)
        {
            case Attack.Lazer:
                StartLazer();
                break;
            
            case Attack.Shoot:
                StartShoot();
                break;
        }
        
    }
    
    public void Pause()
    {
        switch (currentAttack)
        {
            case Attack.Lazer:
                StopLazer();
                break;
            
            case Attack.Shoot:
                StopShoot();
                break;
        }
    }

    public void SendInput(Attack attack)
    {
        attackFSM.SendInput(attack);
    }
}
