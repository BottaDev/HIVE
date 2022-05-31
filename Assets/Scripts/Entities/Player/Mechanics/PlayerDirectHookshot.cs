using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEngine.Serialization;

public class PlayerDirectHookshot : UnlockableMechanic
{
    public enum HookType
    {
        Direct, Indirect
    }
    
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
    [FormerlySerializedAs("grappleable")]
    [SerializeField] private LayerMask directGrappleable;
    [SerializeField] private LayerMask indirectGrappleable;
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

    private HookType _type;
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
        if (_hook == null && player.input.DirectGrapple)
        {
            if (_currentCooldown > 0)
            {
                //Still on cd
                EventManager.Instance.Trigger("OnSendUIMessageTemporary", 
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
        
        //Rotation stuff
        player.model.transform.LookAt(_hook.transform);
        player.model.transform.Rotate(new Vector3(90,0,0));

        if (distance <= stopDistance && useStopDistance)
        {
            DestroyHook();
            _onProximity?.Invoke();
            rigid.velocity = Vector3.zero;
        }
        else
        {
            Vector3 hookDir = (_hook.transform.position - transform.position).normalized;

            

            switch (_type)
            {
                case HookType.Direct:
                    Vector3 addSpeedDirect = hookDir * pullSpeed * Time.deltaTime;
                    float distanceVelocity = Mathf.Clamp(distance, minDistancePullMultiplier, maxDistancePullMultiplier);
                    addSpeedDirect *= distanceVelocity;
                    rigid.position += addSpeedDirect;
                    break;
                case HookType.Indirect:
                    Vector3 addSpeedIndirect = hookDir * pullSpeed * 5 * Time.deltaTime;
                    rigid.AddForce(addSpeedIndirect, ForceMode.VelocityChange);
                    break;
            }
        }
    }

    public void StartPull(HookType type, Action onProximity = null)
    {
        _type = type;
        _onProximity = onProximity;
        _pulling = true;
    }
    
    private void StartHook()
    {
        if(!player.energy.TakeEnergy(energyCost))
        {
            EventManager.Instance.Trigger("OnSendUIMessageTemporary", energyErrorMessage, energyErrorMessageColor, energyErrorTimeOnScreen);
            return;
        }
        player.view.anim.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsHooking, true);
        _pulling = false;
        _hook = Instantiate(hookPrefab, shootTransform.position, Quaternion.identity)
            .GetComponent<PlayerHookObject>();
        
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerHook));
        
        _hook.Initialize(shootTransform, this, player.shoot.defaultAim.Aim(), directGrappleable, indirectGrappleable, hookSpeed, delegate
        {
            //Player settings stuff
            player.movement.useLook = false;
            player.movement.ableToMove = false;
            player.movement.ApplyGravity(false);
            player.jump.CustomAbleToJump = true;


            //Joint stuff
            _joint = gameObject.AddComponent<DistanceJoint3D>();
            _joint.connected = _hook.GetComponent<Rigidbody>();
            _joint.self = rigid;
            _joint.spring = 0;
            _joint.damper = 0;
            _joint.ableToExpand = false;
            _joint.ableToShrink = true;

            if (useMinDistanceToHook)
            {
                _joint.useMinDistance = true;
                _joint.minDistance = minDistance;
            }

            switch (_type)
            {
                case HookType.Direct:
                    rigid.velocity = Vector3.zero;
                    break;
                case HookType.Indirect:
                    break;
            }
            
            
        });
    }
    public void DestroyHook()
    {
        if (_hook == null)
        {
            return;
        }

        player.view.anim.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsHooking, false);
        EventManager.Instance.Trigger("OnPlayerHookshotCD", cooldown);
        _currentCooldown = cooldown;
        
        player.model.transform.localRotation = Quaternion.identity;
        
        player.movement.useLook = true;
        player.movement.ableToMove = true;
        player.movement.ApplyGravity(true);
        player.jump.CustomAbleToJump = false;

        rigid.velocity.Set(rigid.velocity.x,0,rigid.velocity.z);
        _pulling = false;
        Destroy(_joint);
        Destroy(_hook.gameObject);
        lr.SetPositions(new Vector3[]{});
        _hook = null;
    }
    
}
