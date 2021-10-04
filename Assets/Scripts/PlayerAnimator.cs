using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _anim;
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsDashing = Animator.StringToHash("isDashing");
    private static readonly int IsShooting = Animator.StringToHash("isShooting");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    
    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Space))
            _anim.SetBool(IsJumping,true);
        else
            _anim.SetBool(IsJumping, false);
        
        if(Input.GetKeyDown(KeyCode.LeftShift))
            _anim.SetBool(IsDashing, true);
        else
            _anim.SetBool(IsDashing, false);
        
        if(Input.GetKey(KeyCode.Mouse0))
            _anim.SetBool(IsShooting, true);
        else
            _anim.SetBool(IsShooting, false);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
            _anim.SetBool(IsRunning, true);
        else
            _anim.SetBool(IsRunning, false);
    }
}
