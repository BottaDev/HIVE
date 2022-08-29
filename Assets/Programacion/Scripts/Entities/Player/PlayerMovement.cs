using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
public class PlayerMovement : MonoBehaviour
{
    [Header("Assignable")]
    [SerializeField] private Player player;
    [SerializeField] private Transform playerCam;
    public Transform playerModel;
    public Rigidbody rb;


    [Header("Rotation and look")]
    [SerializeField] private Transform legModel;
    [SerializeField] private float sensitivity = 50f;


    [Header("Ground Check")]
    public bool grounded;
    public float groundOffDelay = 3f;
    public LayerMask groundMask;


    [Header("Movement")]
    public bool ableToMove;
    public bool ableToControl;
    [SerializeField] private float moveSpeed = 4500;
    public float maxSpeed = 20;
    public float airMovementMultiplier = 0.5f;
    [HideInInspector] public bool movementDirection;
    public float counterMovement = 0.175f; //Multiplier of velocity for counter movement
    private float originalCounterMovement;
    public float maxSlopeAngle = 35f;


    [Header("Steps")]
    public LayerMask stepMask;
    public float stepCheckDistance = 0.2f;
    [SerializeField] private int stepSmoothing = 5;


    [Header("HorizontalStep")]
    public Transform horizontalStepLower;
    public Transform horizontalStepUpper;
    public Transform horizontalStepMiddle;
    [SerializeField] private float horizontalStepHeight = 0.25f;


    [Header("Debug")]
    public Vector3 currentSpeed;
    public float currentSpeedMagnitude;
    public bool useCounterMovement;
    public bool useExtraGravity;
    public bool addForceX;
    public bool addForceY;
    public bool useLook;
    public bool stepCheck;
    public bool rotateLowerHalf;
    public bool testingNewSystem;
    private readonly float sensMultiplier = 1f;
    private readonly float threshold = 0.01f; //Threshold for speed magnitude before counter movement
    private bool _cancellingGrounded;
    private float _desiredX;
    private float _xRotation;

    //Get info from scripts
    private float X { get => player.input.X; set => player.input.X = value; }

    private float Y { get => player.input.Y; set => player.input.Y = value; }

    public void Start()
    {
        SetMaxSpeed(maxSpeed);
        ableToMove = true;
        horizontalStepUpper.localPosition = horizontalStepLower.localPosition + new Vector3(0, horizontalStepHeight, 0);
        originalCounterMovement = counterMovement;
        EventManager.Instance.Subscribe("GamePause", Pause);
        EventManager.Instance.Subscribe("GameUnPause", Unpause);
    }

    private void Update()
    {
        if (useLook)
        {
            Look();
        }

        Vector3 velocity = rb.velocity;
        currentSpeed = velocity;
        currentSpeedMagnitude = velocity.magnitude;
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
        else
        {
            player.view.anim.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsRunning, false);
        }
    }

    /// <summary>
    ///     Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (groundMask != (groundMask | (1 << layer)))
        {
            return;
        }

        //Iterate through every collision in a physics update
        for (int i = 0;
             i < other.contactCount;
             i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                _cancellingGrounded = false;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        if (!_cancellingGrounded)
        {
            _cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * groundOffDelay);
        }
    }

    private void Movement()
    {
        float x = UIPauseMenu.paused || !ableToControl ? 0 : this.X;
        float y = UIPauseMenu.paused || !ableToControl ? 0 : this.Y;
        
        //Check if you're moving  to set animation (use original input)
        player.view.anim.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsRunning, x != 0 || y != 0);
        
        if (rotateLowerHalf)
        {
            //Rotate towards input movement
            float rotation = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
            legModel.eulerAngles = transform.eulerAngles + new Vector3(0, rotation, 0);
        }

        //Some extra gravity for ground check to work properly
        if (useExtraGravity)
        {
            Vector3 originalSpeed = rb.velocity;
            Vector3 extraGravity = Vector3.down * (10 * Time.deltaTime);
            rb.AddForce(extraGravity, ForceMode.Force);

            rb.velocity = new Vector3(originalSpeed.x, rb.velocity.y, originalSpeed.z);
        }

        
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x;
        float yMag = mag.y;


        //Counteract movement so if you're moving not towards where you're looking, you'll gradually stop
        if (useCounterMovement)
        {
            CounterMovement(x, y, mag);
        }

        /*
        //Limits speed to max speed
        if (x > 0 && xMag > maxSpeed)
        {
            x = 0;
        }

        if (x < 0 && xMag < -maxSpeed)
        {
            x = 0;
        }

        if (y > 0 && yMag > maxSpeed)
        {
            y = 0;
        }

        if (y < 0 && yMag < -maxSpeed)
        {
            y = 0;
        }*/
        
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


        if (addForceX)
        {
            rb.AddForce(xForce * totalMultiplier, ForceMode.Force);
        }

        if (addForceY)
        {
            rb.AddForce(yForce * totalMultiplier, ForceMode.Force);
        }

        Y = y;
        X = x;
        
        ControlMaxSpeed();
    }

    private void ControlMaxSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }
    
    private void StepClimb()
    {
        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (velocity.x != 0 || velocity.z != 0)
        {
            //horizontalStepLower.LookAt(horizontalStepLower.position + velocity);
            //horizontalStepUpper.LookAt(horizontalStepUpper.position + velocity);

            Vector3[] checkArray = {Vector3.forward, new Vector3(1.5f, 0f, 1f), new Vector3(-1.5f, 0f, 1f)};

            foreach (Vector3 check in checkArray)
            {
                Vector3 lowerStart = horizontalStepLower.position;
                Vector3 lowerEnd = horizontalStepLower.TransformDirection(check);

                if (Physics.Raycast(lowerStart, lowerEnd.normalized, out RaycastHit _, stepCheckDistance, stepMask))
                {
                    Vector3 upperStart = horizontalStepUpper.position;
                    Vector3 upperEnd = horizontalStepUpper.TransformDirection(check);

                    if (!Physics.Raycast(upperStart, upperEnd.normalized, out RaycastHit _, stepCheckDistance, stepMask))
                    {
                        for (int i = 0;
                             i <= stepSmoothing;
                             i++)
                        {
                            float currentHeight = i * (horizontalStepHeight / stepSmoothing);
                            horizontalStepMiddle.position = lowerStart + new Vector3(0, currentHeight, 0);
                            Vector3 newPosDirection = horizontalStepMiddle.TransformDirection(check);

                            if (!Physics.Raycast(horizontalStepMiddle.position, newPosDirection.normalized, out RaycastHit _,
                                    stepCheckDistance, stepMask))
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

    public void ApplyImpulseForce(Vector3 force, float groundDisableTime)
    {
        StartCoroutine(ImpulseForce(force, groundDisableTime));
    }

    IEnumerator ImpulseForce(Vector3 force, float groundDisableTime)
    {
        counterMovement = 0;
        rb.AddForce(force, ForceMode.Impulse);
        
        yield return new WaitForSeconds(groundDisableTime);
        
        counterMovement = originalCounterMovement;
    }

    /// <summary>
    ///     Sets rotations of player camera and of the player model towards wherever you were looking
    /// </summary>
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        _desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        //Perform the rotations
        playerModel.transform.localRotation = Quaternion.Euler(0, _desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (testingNewSystem)
        {
            if (grounded && !player.jump.currentlyJumping)
            {
                rb.drag = counterMovement;
            }
            else
            {
                rb.drag = 0;
            }
        }
        else
        {
            if (!grounded || player.jump.currentlyJumping)
            {
                return;
            }
/*
            //Slow down sliding
            if (player.slide.sliding) {
                rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * player.slide.slideCounterMovement);
                return;
            }*/
        
            //Counter movement
            if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || mag.x < -threshold && x > 0 ||
                mag.x > threshold && x < 0)
            {
                rb.AddForce(moveSpeed * playerModel.transform.right * Time.deltaTime * -mag.x * counterMovement);
            }

            if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || mag.y < -threshold && y > 0 ||
                mag.y > threshold && y < 0)
            {
                rb.AddForce(moveSpeed * playerModel.transform.forward * Time.deltaTime * -mag.y * counterMovement);
            }

            //Bunch of math to basically limit diagonal input velocity.
            if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2)) > maxSpeed)
            {
                Vector3 velocity = rb.velocity;
                float fallingSpeed = velocity.y;
                Vector3 n = velocity.normalized * maxSpeed;
                velocity = new Vector3(n.x, fallingSpeed, n.z);
                rb.velocity = velocity;
            }
        }

    }

    /// <summary>
    ///     Find the velocity relative to where the player is looking
    /// </summary>
    private Vector2 FindVelRelativeToLook()
    {
        float lookAngle = playerModel.transform.eulerAngles.y;
        Vector3 velocity = rb.velocity;
        float moveAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private void StopGrounded()
    {
        grounded = false;
    }

    public void SetMaxSpeed(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
        GameStats.movementSPD = maxSpeed;
        UIExtraInfoScreen.i.UpdateStats();
    }

    public void Pause(params object[] obj)
    {
        
    }
    
    public void Unpause(params object[] obj)
    {
        
    }
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(PlayerMovement))]
public class KamCustomEditor_PlayerMovement : KamCustomEditor
{
    private PlayerMovement editorTarget;
    private void OnEnable()
    {
        editorTarget = (PlayerMovement)target;
    }

    public override void GameDesignerInspector()
    {
        EditorGUILayout.LabelField("Movement Variables", EditorStyles.centeredGreyMiniLabel);
        
        editorTarget.groundMask = EditorGUILayout.MaskField(
            new GUIContent("Ground Mask",
                "These are the layers that the player counts as ground. Without this, the player wont reset their jumps, nor walk properly on them."),
            InternalEditorUtility.LayerMaskToConcatenatedLayersMask(editorTarget.groundMask), InternalEditorUtility.layers);
        
        editorTarget.maxSpeed = EditorGUILayout.FloatField(
            new GUIContent(
                "Max Speed",
                "This is the maximum speed the player can reach by moving horizontally"),
            editorTarget.maxSpeed);
        
        editorTarget.airMovementMultiplier = EditorGUILayout.FloatField(
            new GUIContent("Air Movement Multiplier",
                "This is a multiplier applied only on Airborn movement, think of it as the amount of air control the player has."),
            editorTarget.airMovementMultiplier);
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Step Variables", EditorStyles.centeredGreyMiniLabel);
        
        editorTarget.stepMask = EditorGUILayout.MaskField(
            new GUIContent("Step Mask",
            "These are the layers that can trigger the step in the code."),
            InternalEditorUtility.LayerMaskToConcatenatedLayersMask(editorTarget.stepMask), InternalEditorUtility.layers);

        editorTarget.stepCheckDistance = EditorGUILayout.FloatField(
            new GUIContent("Step Height",
                "Player's feet height + Step Height = Max amount a step can be counted as."),
            editorTarget.stepCheckDistance);
        
        editorTarget.stepCheck = EditorGUILayout.Toggle(
            new GUIContent("Activate Step",
                "Activates or deactivates the step feature in the player."),
            editorTarget.stepCheck);
    }
}
#endif
#endregion