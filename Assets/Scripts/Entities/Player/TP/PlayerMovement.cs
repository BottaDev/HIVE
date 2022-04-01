using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header("Assignables")]
    [SerializeField] private PlayerInput    input;
    [SerializeField] private Transform      playerCam;
                     public  Transform      playerModel;
                     public  Rigidbody      rb;

    [Header("Rotation and look")]
    private float xRotation;
    [SerializeField] private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4500;
    public float maxSpeed = 20;
    
    public bool grounded;
    public LayerMask groundMask;
    [HideInInspector] public bool movementDirection;
    [HideInInspector] public bool ableToMove;
    
    public float counterMovement = 0.175f; //Multiplier of velocity for counter movement
    private float threshold = 0.01f; //Threshhold for speed magnitude before counter movement
    public float maxSlopeAngle = 35f;

    [Header("Steps")]
    public Transform stepLower;
    public Transform stepUpper;
    [SerializeField] private float stepHeight = 0.25f;
    [SerializeField] private float stepSmooth = 0.1f;


    [Header("Jumping")]
    public float jumpForce = 550f;
    public float airMovementMultiplier = 0.5f;
    public float fallingGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2f;
    [Tooltip("Time you're still allowed to jump after falling")]
    public float coyoteTime = 0.5f;
    private float coyoteTimeCounter;
    [Tooltip("Time you're still allowed to input a jump before touching the ground")]
    public float jumpBufferingTime = 0.5f;
    private float jumpBufferingCounter;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    private Vector3 normalVector = Vector3.up;

    [Header("Debug")]
    public Vector3 currentSpeed;
    public float currentSpeedMagnitude;

    public void Start()
    {
        ableToMove = true;
        stepUpper.localPosition = new Vector3(0, -1 + stepHeight, 0);
        stepLower.localPosition = new Vector3(0, -1, 0);
    }

    private void FixedUpdate() 
    {
        if (ableToMove)
        {
            Movement();
            JumpCheck();
            //StepClimb();
        }
    }

    private void Update() 
    {
        Look();
        currentSpeed = rb.velocity;
        currentSpeedMagnitude = rb.velocity.magnitude;
    }
    
    private void Movement() 
    {
        //Movement input parameters (just so i don't have to repeat input.x input.y)
        float x = input.x;
        float y = input.y;

        /*
        //Some extra gravity for ground check to work properly
        Vector3 gravityForce = (Vector3.down * 10) * Time.deltaTime;
        rb.AddForce(gravityForce);*/

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x;
        float yMag = mag.y;

        //Counteract movement so if you're moving not towards where you're looking, you'll gradually stop
        CounterMovement(x, y, mag);

        
        
        //Limits speed to max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Multipliers for movement
        float multiplier = 1f;

        //Modify for air movement
        if (!grounded) 
        {
            multiplier = airMovementMultiplier;
        }

        float totalMultiplier = moveSpeed * Time.deltaTime * multiplier;

        //Set forces in each direction
        Vector3 xForce = playerModel.transform.right * x ;
        Vector3 yForce = playerModel.transform.forward * y;
        rb.AddForce(xForce * totalMultiplier);
        rb.AddForce(yForce * totalMultiplier);
    }

    private void JumpCheck() 
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
        if (input.jumping)
        {
            jumpBufferingCounter = jumpBufferingTime;
        }
        else
        {
            jumpBufferingCounter -= Time.deltaTime;
        }


        if (jumpBufferingCounter > 0f && coyoteTimeCounter > 0f && readyToJump)
        {
            Jump(normalVector * jumpForce);
            jumpBufferingCounter = 0;
        }

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * fallingGravityMultiplier * Time.deltaTime;
        }
        else if (input.stoppedJumping && rb.velocity.y > 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * lowJumpGravityMultiplier * Time.deltaTime;
            coyoteTimeCounter = 0;
        }
    }

    public void Jump(Vector3 force)
    {
        readyToJump = false;

        //Reset Y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //Add jump forces
        rb.AddForce(normalVector * jumpForce);

        Invoke(nameof(ResetJump), jumpCooldown);
    }
    
    private void ResetJump() 
    {
        readyToJump = true;
    }
    
    public void StepClimb()
    {
        Vector3[] checkArray = { Vector3.forward, new Vector3(1.5f,0f,1f), new Vector3(-1.5f, 0f, 1f) };

        foreach (Vector3 check in checkArray)
        {
            RaycastHit lower;
            if (Physics.Raycast(stepLower.position, transform.TransformDirection(check), out lower, 0.1f, groundMask))
            {
                Debug.Log("Lower collision");
                RaycastHit upper;
                if (!Physics.Raycast(stepUpper.position, transform.TransformDirection(check), out upper, 0.2f, groundMask))
                {
                    Debug.Log("Step up");
                    rb.position += new Vector3(0f, stepSmooth, 0f);
                }
            }
        }
    }
    public void ApplyGravity(bool setting)
    {
        rb.useGravity = setting;
    }

    private float desiredX;

    /// <summary>
    /// Sets rotations of player camera and of the player model towards wherever you were looking
    /// </summary>
    private void Look() 
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerModel.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag) 
    {
        if (!grounded || input.jumping) return;

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) 
        {
            rb.AddForce(moveSpeed * playerModel.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }

        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * playerModel.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        
        //Bunch of math to basically limit diagonal input velocity.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) 
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// </summary>
    public Vector2 FindVelRelativeToLook() 
    {
        float lookAngle = playerModel.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) 
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;
    
    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other) 
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (groundMask != (groundMask | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }
    
}
