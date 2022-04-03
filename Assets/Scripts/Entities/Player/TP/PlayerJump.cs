using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerMovement movement;

    //Variables taken out of the scripts
    public bool grounded { get { return movement.grounded; } }
    public Rigidbody rb { get { return movement.rb; } }
    public bool jumping { get { return input.jumping; } }
    public bool stoppedJumping { get { return input.stoppedJumping; } }

    [Header("Jumping")]
    public float jumpForce = 550f;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    private Vector3 normalVector = Vector3.up;
    [SerializeField] public bool currentlyJumping;

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
    public bool fallingGravity;
    public bool lowJumpGravity;
    public bool jumpBufferCondition;
    public bool coyoteTimeCondition;
    public bool readyToJumpCondition;
    public bool lowJumpCondition;
    private void Update()
    {
        //CoyoteTime
        if (grounded)
        {
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
        if (jumpBufferCondition && coyoteTimeCondition && readyToJump && !currentlyJumping)
        {
            Jump(normalVector * jumpForce);
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
            lowJumpCondition = false;
            currentlyJumping = false;
            fallingGravity = true;
            rb.velocity += Vector3.up * Physics.gravity.y * fallingGravityMultiplier * Time.deltaTime;
        }
    }

    public void Jump(Vector3 force)
    {
        readyToJump = false;
        currentlyJumping = true;

        //Reset Y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //Add jump forces
        rb.AddForce(force);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
