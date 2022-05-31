using UnityEngine;
using UnityEngine.Serialization;

public class PlayerJump : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Player player;

    [Header("Jumping")]
    public int amountOfJumps = 1;
    public float jumpForce = 550f;
    [SerializeField] public bool currentlyJumping;

    [Header("Multipliers")]
    public float fallingGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2f;

    [Header("Parameters")]
    [Tooltip("Time you're still allowed to jump after falling")]
    public float coyoteTime = 0.5f;

    [Tooltip("Time you're still allowed to input a jump before touching the ground")]
    public float jumpBufferingTime = 0.5f;
    private bool _coyoteTimeCondition;
    private float _coyoteTimeCounter;


    [Header("Debug")] 
    public bool fallingGravity;
    public bool lowJumpGravity;
    private bool _firstJump;
    private bool _jumpBufferCondition;
    private float _jumpBufferingCounter;
    private readonly float jumpCooldown = 0.25f;
    private int _jumpCounter;
    private bool _lowJumpCondition;
    public bool CustomAbleToJump { get; set; }
    private readonly Vector3 _normalVector = Vector3.up;
    private bool _readyToJump = true;
    private bool _readyToJumpCondition;

    //Variables taken out of the scripts
    private bool Grounded => player.movement.grounded;
    private Rigidbody Rb => player.movement.rb;
    private bool Jumping => player.input.Jumping;
    private bool StoppedJumping => player.input.StoppedJumping;

    private void Update()
    {
        //CoyoteTime
        if (Grounded || CustomAbleToJump)
        {
            if (!currentlyJumping)
            {
                _jumpCounter = amountOfJumps;
                _firstJump = true;
            }

            _coyoteTimeCounter = coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        //Jump buffering
        if (Jumping)
        {
            _jumpBufferingCounter = jumpBufferingTime;
        }
        else
        {
            _jumpBufferingCounter -= Time.deltaTime;
        }

        //Low jump
        if (StoppedJumping)
        {
            _lowJumpCondition = true;
        }
    }

    private void FixedUpdate()
    {
        JumpCheck();
    }

    private void JumpCheck()
    {
        _jumpBufferCondition = _jumpBufferingCounter > 0f;
        _coyoteTimeCondition = _coyoteTimeCounter > 0f;
        _readyToJumpCondition = _readyToJump;
        lowJumpGravity = false;
        fallingGravity = false;

        //Jump action
        if (_jumpBufferCondition && _readyToJumpCondition && _jumpCounter > 0 && !player.hookshot.Pulling)
        {
            //If you don't jump within coyote time in your first jump, it counts as 2 jumps (so you lose one jump if you do it from the air)
            if (!_coyoteTimeCondition && _firstJump)
            {
                _jumpCounter -= 2;
            }
            else
            {
                _jumpCounter -= 1;
            }

            Jump(_normalVector * jumpForce);
            _firstJump = false;
            _lowJumpCondition = false;
            _jumpBufferingCounter = 0;
        }

        //Low jump
        if (_lowJumpCondition && Rb.velocity.y > 0 && player.movement.rb.useGravity)
        {
            lowJumpGravity = true;
            Rb.velocity += Vector3.up * Physics.gravity.y * lowJumpGravityMultiplier * Time.deltaTime;
            _coyoteTimeCounter = 0;
        }

        //Falling = higher gravity
        if (!Grounded)
        {
            if (Rb.velocity.y < 0 && player.movement.rb.useGravity)
            {
                player.view.anim.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsJumping, false);
                _lowJumpCondition = false;
                currentlyJumping = false;
                fallingGravity = true;
                Rb.velocity += Vector3.up * Physics.gravity.y * fallingGravityMultiplier * Time.deltaTime;
            }
        }

    }

    public void Jump(Vector3 force)
    {
        currentlyJumping = true;
        
        player.hookshot.DestroyHook();
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerJump));


        //Reset Y velocity
        Rb.velocity = new Vector3(Rb.velocity.x, 0, Rb.velocity.z);

        //Add jump forces
        Rb.AddForce(force);

        player.view.anim.AnimationBooleans(PlayerAnimator.AnimationTriggers.IsJumping, true);

        _readyToJump = false;
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        _readyToJump = true;
        currentlyJumping = false;
    }
    
    private void ResetCurrentlyJumping()
    {
        currentlyJumping = false;
    }
}