using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class PlayerDirectHookshot : UnlockableMechanic
{
    public enum HookType
    {
        Direct, Indirect
    }
    
    [Header("Energy")]
    public float energyCost = 0;
    [SerializeField] private string energyErrorMessage;
    [SerializeField] private Color energyErrorMessageColor;
    [SerializeField] private float energyErrorTimeOnScreen;

    [Header("Cooldown")]
    public float cooldown;
    [SerializeField] private string cooldownErrorMessage;
    [SerializeField] private Color cooldownErrorMessageColor;
    [SerializeField] private float cooldownErrorTimeOnScreen;
    private float _currentCooldown;
    
    
    [Header("Parameters")]
    [FormerlySerializedAs("grappleable")]
    public LayerMask directGrappleable;
    public LayerMask indirectGrappleable;
    public LayerMask railMask;
    public float maxHookDistance = 50f;
    public float hookSpeed = 5f;
    public float pullSpeed = 0.5f;
    public float minDistancePullMultiplier = 0.5f;
    public float maxDistancePullMultiplier = 10f;
    public float minDistance = 1f;
    
    [Header("Assignables")]
    [SerializeField] private Player player;
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private LineRenderer lr;

    [Header("Break distance")]
    public bool useStopDistance;
    public float stopDistance = 4f;
    
    [Header("Settings")]
    public bool sameButtonPressCancel;
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
        
        _hook.Initialize(shootTransform, this, player.shoot.defaultAim.Aim(), directGrappleable, indirectGrappleable, railMask, hookSpeed, delegate
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

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(PlayerDirectHookshot))]
public class KamCustomEditor_PlayerDirectHookshot : KamCustomEditor
{
    private PlayerDirectHookshot editorTarget;
    private void OnEnable()
    {
        editorTarget = (PlayerDirectHookshot)target;
    }
    
    public override void GameDesignerInspector()
    {
        editorTarget.unlockedAtTheStart = EditorGUILayout.Toggle(
            new GUIContent("Start Unlock",
                "This boolean determines if this is unlocked by default."),
            editorTarget.unlockedAtTheStart);
        
        EditorGUILayout.LabelField("General", EditorStyles.centeredGreyMiniLabel);

        editorTarget.energyCost = EditorGUILayout.FloatField(
            new GUIContent(
                "Cost",
                "This is the energy cost of casting dash."),
            editorTarget.energyCost);
        
        editorTarget.cooldown = EditorGUILayout.FloatField(
            new GUIContent(
                "Cooldown",
                "Time it takes for hook to be able to be cast again. It starts the moment the hook comes back to you."),
            editorTarget.cooldown);
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Layer Masks", EditorStyles.centeredGreyMiniLabel);
        
        editorTarget.directGrappleable = EditorGUILayout.MaskField(
            new GUIContent("Direct Mask",
                "This mask is what layers will trigger the direct hookshot."),
            InternalEditorUtility.LayerMaskToConcatenatedLayersMask(editorTarget.directGrappleable), InternalEditorUtility.layers);
        
        editorTarget.indirectGrappleable = EditorGUILayout.MaskField(
            new GUIContent("Indirect Mask",
                "This mask is what layers will trigger the direct hookshot."),
            InternalEditorUtility.LayerMaskToConcatenatedLayersMask(editorTarget.indirectGrappleable), InternalEditorUtility.layers);
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Parameters", EditorStyles.centeredGreyMiniLabel);
        
        editorTarget.maxHookDistance = EditorGUILayout.FloatField(
            new GUIContent(
                "Max Distance",
                "This is the max distance the hook can go away from player before destroying itself."),
            editorTarget.maxHookDistance);
        
        editorTarget.hookSpeed = EditorGUILayout.FloatField(
            new GUIContent(
                "Hook Speed",
                "The speed the hook travels at."),
            editorTarget.hookSpeed);

        EditorGUILayout.BeginHorizontal();
        editorTarget.stopDistance = EditorGUILayout.FloatField(new GUIContent(
            "Break distance",
            "The distance at which the hook considers you've arrived. The boolean is if the hook uses the stop distance or not, in case you don't want it to stop at all."),
            editorTarget.stopDistance);
        editorTarget.useStopDistance = EditorGUILayout.Toggle(editorTarget.useStopDistance, GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();
        editorTarget.sameButtonPressCancel = EditorGUILayout.Toggle(
            new GUIContent("Double Press Cancel",
                "This boolean determines if pressing the hook button again will cancel the hook. In case of being false, it'll cancel when you let go of the button."),
            editorTarget.sameButtonPressCancel);
    }
}
#endif
#endregion