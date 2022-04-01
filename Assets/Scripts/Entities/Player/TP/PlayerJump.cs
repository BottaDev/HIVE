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

    [Header("Jumping")]
    public float jumpForce = 550f;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    private Vector3 normalVector = Vector3.up;

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
    public bool jumpBufferCondition;
    public bool coyoteTimeCondition;
    public bool readyToJumpCondition;

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
    }

    private void FixedUpdate()
    {
        if (movement.ableToMove)
        {
            JumpCheck();
        }
    }


    private void JumpCheck()
    {
        jumpBufferCondition = jumpBufferingCounter > 0f;
        coyoteTimeCondition = coyoteTimeCounter > 0f;
        readyToJumpCondition = readyToJump;

        if (jumpBufferCondition && coyoteTimeCondition && readyToJump)
        {
            Jump(normalVector * jumpForce);
            jumpBufferingCounter = 0;
        }
        else
        {
            if (input.stoppedJumping && rb.velocity.y > 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * lowJumpGravityMultiplier * Time.deltaTime;
                coyoteTimeCounter = 0;
            }
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * fallingGravityMultiplier * Time.deltaTime;
        }
    }

    public void Jump(Vector3 force)
    {
        readyToJump = false;

        //Reset Y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //Just for debugging
        rb.velocity = new Vector3(0, 0, 0);

        //Add jump forces
        rb.AddForce(force);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
