using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimator : MonoBehaviour
{
    public enum AnimationTriggers
    {
        IsJumping, IsDashing, IsShooting, IsRunning, IsHooking
    }

    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsDashing = Animator.StringToHash("isDashing");
    private static readonly int IsShooting = Animator.StringToHash("isShooting");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsHooking = Animator.StringToHash("isHooking");
    [FormerlySerializedAs("_top")] [SerializeField] private Animator top;
    [FormerlySerializedAs("_bottom")] [SerializeField] private Animator bottom;

    public void AnimationBooleans(AnimationTriggers trigger, bool state)
    {
        switch (trigger)
        {
            case AnimationTriggers.IsJumping:
                top.SetBool(IsJumping, state);
                bottom.SetBool(IsJumping, state);
                break;
            case AnimationTriggers.IsDashing:
                top.SetBool(IsDashing, state);
                bottom.SetBool(IsDashing, state);
                break;
            case AnimationTriggers.IsShooting:
                top.SetBool(IsShooting, state);
                break;
            case AnimationTriggers.IsRunning:
                top.SetBool(IsRunning, state);
                bottom.SetBool(IsRunning, state);
                break;
            case AnimationTriggers.IsHooking:
                top.SetBool(IsHooking, state); 
                break;
        }
    }
}