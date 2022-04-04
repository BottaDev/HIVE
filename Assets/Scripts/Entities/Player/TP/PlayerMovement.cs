using System;
using System.Collections;
using UnityEngine;
public class PlayerMovement : MonoBehaviour {

    [Header("Assignables")]
    [SerializeField] private PlayerInput    input;
    [SerializeField] private Transform      playerCam;
                     public  Transform      playerModel;
                     public  PlayerJump     jump;
                     public  Rigidbody      rb;

    //Get info from scripts
    public float x { get { return input.x; } set { input.x = value; } }
    public float y { get { return input.y; } set { input.y = value; } }

    [Header("Rotation and look")]
    private float xRotation;
    [SerializeField] private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    [Header("Ground Check")]
    public bool grounded;
    public float groundOffDelay = 3f;
    public LayerMask groundMask;


    [Header("Movement")]
    public bool ableToMove;
    [SerializeField] private float moveSpeed = 4500;
    public float maxSpeed = 20;
    
    
    public float airMovementMultiplier = 0.5f;
    [HideInInspector] public bool movementDirection;
    public float counterMovement = 0.175f; //Multiplier of velocity for counter movement
    private float threshold = 0.01f; //Threshhold for speed magnitude before counter movement
    public float maxSlopeAngle = 35f;

    [Header("Steps")]
    public LayerMask stairsMask;
    public Transform stepLower;
    public Transform stepUpper;
    public Transform stepMiddle;
    [SerializeField] private float stepHeight = 0.25f;
    [SerializeField] private float stepSmooth = 0.1f;
    [SerializeField] private int stepSmoothing = 5;
    [SerializeField] private float stepCheckDistance = 0.2f;

    [Header("Debug")]
    public Vector3 currentSpeed;
    public float currentSpeedMagnitude;
    public bool useCounterMovement;
    public bool useExtraGravity;
    public bool addForceX;
    public bool addForceY;
    public bool useLook;
    public bool stepCheck;

    public void Start()
    {
        ableToMove = true;
        stepUpper.localPosition = stepLower.localPosition + new Vector3(0, stepHeight, 0);
    }

    private void FixedUpdate() 
    {
        if (ableToMove)
        {
            Movement();

            if (stepCheck)
            {
                StepClimb();
            }
        }
    }

    private void Update() 
    {
        if(useLook)
        Look();
        currentSpeed = rb.velocity;
        currentSpeedMagnitude = rb.velocity.magnitude;
    }
    private void Movement() 
    {
        //Some extra gravity for ground check to work properly
        if (useExtraGravity)
        {
            Vector3 originalSpeed = rb.velocity; 
            Vector3 extraGravity = (Vector3.down * 10) * Time.deltaTime;
            rb.AddForce(extraGravity);

            rb.velocity = new Vector3(originalSpeed.x, rb.velocity.y, originalSpeed.z);
        }

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x;
        float yMag = mag.y;

     
        //Counteract movement so if you're moving not towards where you're looking, you'll gradually stop
        if (useCounterMovement)
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
        Vector3 xForce = playerModel.transform.right * x;
        Vector3 yForce = playerModel.transform.forward * y;

        

        if(addForceX)
            rb.AddForce(xForce * totalMultiplier);
        if(addForceY)
            rb.AddForce(yForce * totalMultiplier);
    }
    public void StepClimb()
    {
        if (rb.velocity.x != 0 || rb.velocity.z != 0)
        {
            Vector3[] checkArray = { Vector3.forward, new Vector3(1.5f, 0f, 1f), new Vector3(-1.5f, 0f, 1f) };

            foreach (Vector3 check in checkArray)
            {
                Vector3 lowerStart = stepLower.position;
                Vector3 lowerEnd = stepLower.TransformDirection(check);

                RaycastHit lower;
                if (Physics.Raycast(lowerStart, lowerEnd.normalized, out lower, stepCheckDistance, stairsMask))
                {
                    Vector3 upperStart = stepUpper.position;
                    Vector3 upperEnd = stepUpper.TransformDirection(check);

                    RaycastHit upper;
                    if (!Physics.Raycast(upperStart, upperEnd.normalized, out upper, stepCheckDistance, stairsMask))
                    {
                        for (int i = 0; i <= stepSmoothing; i++)
                        {
                            float currentHeight = i * (stepHeight / stepSmoothing);
                            stepMiddle.position = lowerStart + new Vector3(0, currentHeight, 0);
                            Vector3 newPosDirection = stepMiddle.TransformDirection(check);

                            if (!Physics.Raycast(stepMiddle.position, newPosDirection.normalized, out upper, stepCheckDistance, stairsMask))
                            {
                                rb.position += new Vector3(0, currentHeight, 0);
                                break;
                            }
                        }
                    }
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
        if (!grounded || jump.currentlyJumping) return;

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) 
        {
            rb.AddForce(moveSpeed * playerModel.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }

        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) 
        {
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
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * groundOffDelay);
        }
    }
    private void StopGrounded() {
        grounded = false;
    }
    
}
