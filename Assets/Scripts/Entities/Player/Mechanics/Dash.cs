using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
public class Dash : UnlockableMechanic
{
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
    [FormerlySerializedAs("_dashVelocity")]
    [Tooltip("Speed during dash")] 
    public float dashVelocity;
    [FormerlySerializedAs("_dashDuration")]
    [Tooltip("Time you stay in dash Velocity.")] 
    public float dashDuration;
    
    [Header("Assignable")]
    [SerializeField] private Player player;
    [SerializeField] private TrailRenderer[] trails;
    private Camera _cam;


    //Get whatever information you need for this script
    private bool Dashing => player.input.Dashing;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if (!mechanicUnlocked) return;
        
        if (Dashing)
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
                if (!player.energy.TakeEnergy(energyCost))
                {
                    EventManager.Instance.Trigger("OnSendUIMessageTemporary", 
                        energyErrorMessage, 
                        energyErrorMessageColor, 
                        energyErrorTimeOnScreen);
                    return;
                }
            
                EventManager.Instance.Trigger("OnPlayerDashCd", cooldown);
                if (player.attachedToRail)
                {
                    StartCoroutine(RailDashCast(player.attachedRail));
                }
                else
                {
                    StartCoroutine(DashCast());
                }
                
                StartCoroutine(CameraEffect());
            }
            
        }

        _currentCooldown -= Time.deltaTime;
    }

    private IEnumerator DashCast()
    {
        player.hookshot.DestroyHook();
        
        foreach (TrailRenderer item in trails)
        {
            item.emitting = true;
        }
        
        _currentCooldown = cooldown;

        bool ableToMoveOld = player.movement.ableToMove;
        bool applyGravityOld = player.movement.rb.useGravity;
        player.movement.ableToMove = false;
        player.movement.ApplyGravity(false);
        
        
        Vector3 originalVelocity = player.movement.rb.velocity;
        Vector3 orientation = new Vector3(originalVelocity.x, 0, originalVelocity.z);
        if (orientation.magnitude == 0)
        {
            orientation = player.movement.playerModel.forward;
        }

        Vector3 dashdirection = orientation.normalized;
        player.movement.rb.velocity = dashdirection * dashVelocity;

        player.view.anim.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsDashing, true);

        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerDash));
    
        yield return new WaitForSeconds(dashDuration);

        player.view.anim.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsDashing, false);

        foreach (TrailRenderer item in trails)
        {
            item.emitting = false;
        }

        dashdirection = player.movement.rb.velocity.normalized;

        player.movement.rb.velocity = dashdirection * originalVelocity.magnitude;

        player.movement.ableToMove = ableToMoveOld;
        player.movement.ApplyGravity(applyGravityOld);
    }
    
    public IEnumerator RailDashCast(Rails rail)
    {
        foreach (TrailRenderer item in trails)
        {
            item.emitting = true;
        }
        
        _currentCooldown = cooldown;
        
        float originalVelocity = rail.speed;
        float originalDetectionRange = rail.detectionRange;
        rail.speed = rail.railDashSpeed;
        rail.detectionRange = Mathf.RoundToInt(rail.railDashSpeed / 7f) * 0.1f;
        player.movement.ableToMove = false;
        
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerDash));
    
        yield return new WaitForSeconds(dashDuration);
        
        foreach (TrailRenderer item in trails)
        {
            item.emitting = false;
        }

        player.movement.ableToMove = true;
        rail.speed = originalVelocity;
        rail.detectionRange = originalDetectionRange;
    }

    private IEnumerator CameraEffect()
    {
        float steps = 10;

        float speedTime = dashDuration / 2f;
        float slowTime = dashDuration - speedTime;

        for (int i = 0;
             i < steps;
             i++)
        {
            _cam.fieldOfView += 1;
            yield return new WaitForSeconds(speedTime / steps);
        }

        for (int i = 0;
             i < steps;
             i++)
        {
            _cam.fieldOfView -= 1;
            yield return new WaitForSeconds(slowTime / steps);
        }
    }
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(Dash))]
public class KamCustomEditor_Dash : KamCustomEditor
{
    private Dash editorTarget;
    private void OnEnable()
    {
        editorTarget = (Dash)target;
    }
    
    public override void GameDesignerInspector()
    {
        editorTarget.unlockedAtTheStart = EditorGUILayout.Toggle(
            new GUIContent("Start Unlock",
                "This boolean determines if this is unlocked by default."),
            editorTarget.unlockedAtTheStart);
        
        EditorGUILayout.LabelField("Parameters", EditorStyles.centeredGreyMiniLabel);

        editorTarget.energyCost = EditorGUILayout.FloatField(
            new GUIContent(
                "Cost",
                "This is the energy cost of casting dash."),
            editorTarget.energyCost);
        
        editorTarget.cooldown = EditorGUILayout.FloatField(
            new GUIContent(
                "Cooldown",
                "Time it takes for dash to be able to be cast again. It starts the moment you press the dash key."),
            editorTarget.cooldown);
        
        editorTarget.dashVelocity = EditorGUILayout.FloatField(
            new GUIContent(
                "Speed",
                "This speed will override the player's speed when dashing, and it will be constant for the duration the dash lasts. Keep in mind changing either the speed or the duration will change how \"Long\" the dash is."),
            editorTarget.dashVelocity);
        
        editorTarget.dashDuration = EditorGUILayout.FloatField(
            new GUIContent(
                "Duration",
                "This is the amount of time the dash will last. For this period of time, the player's speed will be equal to the dash's speed. Keep in mind changing either the speed or the duration will change how \"Long\" the dash is."),
            editorTarget.dashDuration);
    }
}
#endif
#endregion