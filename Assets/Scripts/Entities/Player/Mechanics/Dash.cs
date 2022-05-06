using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Dash : UnlockableMechanic
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
    [FormerlySerializedAs("_dashVelocity")]
    [Tooltip("Speed during dash")] 
    [SerializeField] private float dashVelocity;
    [FormerlySerializedAs("_dashDuration")]
    [Tooltip("Time you stay in dash Velocity.")] 
    [SerializeField] private float dashDuration;
    
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

        player.animator.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsDashing, true);

        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerDash));
    
        yield return new WaitForSeconds(dashDuration);

        player.animator.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsDashing, false);

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