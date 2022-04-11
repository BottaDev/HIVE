using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Player player;

    //Variables taken out of the scripts
    public bool grounded { get { return player.movement.grounded; } }
    public Rigidbody rb { get { return player.movement.rb; } }
    public bool jumping { get { return player.input.jumping; } }
    public bool stoppedJumping { get { return player.input.stoppedJumping; } }

    [Header("Jumping")]
    public int amountOfJumps = 1;
    public float jumpForce = 550f;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    private Vector3 normalVector = Vector3.up;
    [SerializeField] public bool currentlyJumping;
    private int jumpCounter;
    private bool firstJump;

    [Header("Multipliers")]
    public float fallingGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2f;
    
    [Header("Parameters")]
    [Tooltip("Time you're still allowed to jump after falling")]
    public float coyoteTime = 0.5f;
    private float coyoteTimeCounter;

    [Tooltip("Time you're still allowed to input a jump before touching the ground")]
    public float jumpBufferingTime = 0.5f;
    private float jumpBufferingCounter;


    [Header("Debug")]
    bool fallingGravity;
    bool lowJumpGravity;
    bool jumpBufferCondition;
    bool coyoteTimeCondition;
    bool readyToJumpCondition;
    bool lowJumpCondition;
    private void Update()
    {
        //CoyoteTime
        if (grounded)
        {
            if (!currentlyJumping)
            {
                jumpCounter = amountOfJumps;
                firstJump = true;
            }

            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //Jump buffering
        if (jumping)
        {
            jumpBufferingCounter = jumpBufferingTime;
        }
        else
        {
            jumpBufferingCounter -= Time.deltaTime;
        }

        //Low jump
        if (stoppedJumping)
        {
            lowJumpCondition = true;
        }
    }

    private void FixedUpdate()
    {
        
        
        JumpCheck();
    }

    private void JumpCheck()
    {
        jumpBufferCondition = jumpBufferingCounter > 0f;
        coyoteTimeCondition = coyoteTimeCounter > 0f;
        readyToJumpCondition = readyToJump;
        lowJumpGravity = false;
        fallingGravity = false;

        //Jump action
        if (jumpBufferCondition && readyToJump && jumpCounter > 0)
        {
            //If you don't jump within coyote time in your first jump, it counts as 2 jumps (so you lose one jump if you do it from the air)
            if (!coyoteTimeCondition && firstJump)
            {
                jumpCounter -= 2;
            }
            else
            {
                jumpCounter -= 1;
            }
            
            Jump(normalVector * jumpForce);
            firstJump = false;
            lowJumpCondition = false;
            jumpBufferingCounter = 0;
        }

        //Low jump
        if (lowJumpCondition && rb.velocity.y > 0)
        {
            lowJumpGravity = true;
            rb.velocity += Vector3.up * Physics.gravity.y * lowJumpGravityMultiplier * Time.deltaTime;
            coyoteTimeCounter = 0;
        }
        
        //Falling = higher gravity
        if (rb.velocity.y < 0 && !grounded)
        {
            player.animator.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsJumping, false);
            lowJumpCondition = false;
            currentlyJumping = false;
            fallingGravity = true;
            rb.velocity += Vector3.up * Physics.gravity.y * fallingGravityMultiplier * Time.deltaTime;
        }
    }

    public void Jump(Vector3 force)
    {   
        currentlyJumping = true;

        //Reset Y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //Add jump forces
        rb.AddForce(force);

        player.animator.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsJumping, true);

        readyToJump = false;
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
