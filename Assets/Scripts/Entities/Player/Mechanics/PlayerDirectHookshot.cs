using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerDirectHookshot : UnlockableMechanic,IGrapple
{
    [Header("Energy")]
    [SerializeField] private float energyCost = 0;
    [SerializeField] private string energyErrorMessage;
    [SerializeField] private Color energyErrorMessageColor;
    [SerializeField] private float energyErrorTimeOnScreen;

    [Header("Cooldown")]
    [SerializeField] private float cooldown;
    [SerializeField] private string cooldownErrorMessage;
    [SerializeField] private Color cooldownErrorMessageColor;
    [SerializeField] private float cooldownErrorTimeOnScreen;
    private float _currentCooldown;
    
    [Header("Parameters")]
    [SerializeField] private LayerMask grappleable;
    [SerializeField] private float maxHookDistance = 50f;
    [SerializeField] private float hookSpeed = 5f;
    [SerializeField] private float pullSpeed = 0.5f;
    [SerializeField] private float minDistancePullMultiplier = 0.5f;
    [SerializeField] private float maxDistancePullMultiplier = 10f;
    [SerializeField] private float minDistance = 1f;
    
    [Header("Assignables")]
    [SerializeField] private Player player;
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private LineRenderer lr;

    [Header("Break distance")]
    [SerializeField] private bool useStopDistance;
    [SerializeField] private float stopDistance = 4f;
    
    [Header("Settings")]
    [SerializeField] private bool sameButtonPressCancel;
    [SerializeField] private bool useMinDistanceToHook;
    [SerializeField] private bool useDistanceMultiplier;
    [SerializeField] private bool useRigidbodyPosition;

    private PlayerHookObject _hook;
    private bool _pulling;
    private Action _onProximity;
    private DistanceJoint3D _joint;

    public bool Pulling => _pulling;

    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        _pulling = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!mechanicUnlocked) return;
        
        _currentCooldown -= Time.deltaTime;
        if (_hook == null && player.input.DirectGrapple && !player.grapple.Pulling)
        {
            if (_currentCooldown > 0)
            {
                //Still on cd
                EventManager.Instance.Trigger(EventManager.Events.OnSendUIMessageTemporary, 
                    cooldownErrorMessage, 
                    cooldownErrorMessageColor, 
                    cooldownErrorTimeOnScreen);
            }
            else
            {
                StartHook();
            }
        }
        else if (_hook != null && (sameButtonPressCancel ? player.input.DirectGrapple : player.input.StoppedDirectGrapple))
        {
            DestroyHook();
        }
        
        if (_hook == null)
        {
            return;
        }
        
        float distance = Vector3.Distance(transform.position, _hook.transform.position);
        if (distance > maxHookDistance)
        {
            DestroyHook();
        }
        
        if (!_pulling) 
        {
            return;
        }

        if (player.input.Dashing || player.input.Jumping)
        {
            DestroyHook();
        }
        else if (distance <= stopDistance && useStopDistance)
        {
            DestroyHook();
            _onProximity?.Invoke();
            rigid.velocity = Vector3.zero;
        }
        else
        {

            Vector3 hookDir = (_hook.transform.position - transform.position).normalized;

            Vector3 addSpeed = hookDir * pullSpeed * Time.deltaTime;

            if (useDistanceMultiplier)
            {
                float distanceVelocity = Mathf.Clamp(distance, minDistancePullMultiplier, maxDistancePullMultiplier);
                addSpeed *= distanceVelocity;
            }

            if (useRigidbodyPosition)
            {
                rigid.position += addSpeed;
            }
            else
            {
                rigid.AddForce(addSpeed, ForceMode.VelocityChange);
            }
            
        }
    }

    public void StartPull(Action onProximity)
    {
        _onProximity = onProximity;
        _pulling = true;
    }

    private void StartHook()
    {
        if(!player.energy.TakeEnergy(energyCost))
        {
            EventManager.Instance.Trigger(EventManager.Events.OnSendUIMessageTemporary, energyErrorMessage, energyErrorMessageColor, energyErrorTimeOnScreen);
            return;
        }

        _pulling = false;
        _hook = Instantiate(hookPrefab, shootTransform.position, Quaternion.identity)
            .GetComponent<PlayerHookObject>();
        
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerHook));
        
        _hook.Initialize(shootTransform, this, player.aim.Point, grappleable, hookSpeed, delegate
        {
            _joint = gameObject.AddComponent<DistanceJoint3D>();
            _joint.connected = _hook.GetComponent<Rigidbody>();
            _joint.self = rigid;
            _joint.spring = 0;
            _joint.damper = 0;
            _joint.ableToExpand = false;
            _joint.ableToShrink = true;

            player.movement.ableToMove = false;
            
            if (useMinDistanceToHook)
            {
                _joint.useMinDistance = true;
                _joint.minDistance = minDistance;
            }

            rigid.velocity = Vector3.zero;
            
            
            player.movement.ApplyGravity(false);
            
        });
    }
    private void DestroyHook()
    {
        if (_hook == null)
        {
            return;
        }

        EventManager.Instance.Trigger(EventManager.Events.OnPlayerDirectHookshotCD, cooldown);
        _currentCooldown = cooldown;
        player.movement.ableToMove = true;
        player.movement.ApplyGravity(true);
        

        rigid.velocity.Set(rigid.velocity.x,0,rigid.velocity.z);
        _pulling = false;
        Destroy(_joint);
        Destroy(_hook.gameObject);
        lr.SetPositions(new Vector3[]{});
        _hook = null;
    }
    
}
