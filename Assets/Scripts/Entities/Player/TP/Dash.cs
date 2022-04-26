using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Dash : MonoBehaviour
{
    [Header("Assignable")]
    [SerializeField] private Player player;
    [SerializeField] private TrailRenderer[] trails;

    
    [Header("Parameters")]
    [Tooltip("Energy cost of dash")]
    [SerializeField] private float energyCost;
    [FormerlySerializedAs("_dashVelocity")]
    [Tooltip("Speed during dash")] 
    [SerializeField] private float dashVelocity;
    [FormerlySerializedAs("_dashDuration")]
    [Tooltip("Time you stay in dash Velocity.")] 
    [SerializeField] private float dashDuration;
    [FormerlySerializedAs("_dashCD")]
    [Tooltip("Time it takes for you to be able to dash again after cast.")] 
    [SerializeField] private float dashCd;
    private Camera _cam;


    private float _currentDashCd;

    //Get whatever information you need for this script
    private bool Dashing => player.input.Dashing;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if (Dashing && _currentDashCd <= 0)
        {
            if (!player.energy.TakeEnergy(energyCost)) return;
            StartCoroutine(Cast());
            StartCoroutine(CameraEffect());
        }

        _currentDashCd -= Time.deltaTime;
    }

    private IEnumerator Cast()
    {
        foreach (TrailRenderer item in trails)
        {
            item.emitting = true;
        }
        
        _currentDashCd = dashCd;

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