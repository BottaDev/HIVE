using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _top;
    [SerializeField] private Animator _bottom;
    
    public enum AnimationTriggers
    {
        IsJumping, IsDashing, IsShooting, IsRunning
    }

    public void AnimationBooleans(AnimationTriggers trigger, bool state)
    {
        switch (trigger)
        {
            case AnimationTriggers.IsJumping:
                _top.SetBool("isJumping", state);
                _bottom.SetBool("isJumping", state);
                break;
            case AnimationTriggers.IsDashing:
                _top.SetBool("isDashing", state);
                _bottom.SetBool("isDashing", state);
                break;
            case AnimationTriggers.IsShooting:
                _top.SetBool("isShooting", state);
                break;
            case AnimationTriggers.IsRunning:
                _top.SetBool("isRunning", state);
                _bottom.SetBool("isRunning", state);
                break;
        }
        
    }
}