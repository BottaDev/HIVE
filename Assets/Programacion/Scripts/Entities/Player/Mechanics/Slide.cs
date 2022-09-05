using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : UnlockableMechanic
{
    [Header("Energy")]
    public float energyCost = 0;
    [SerializeField] private string energyErrorMessage;
    [SerializeField] private Color energyErrorMessageColor;
    [SerializeField] private float energyErrorTimeOnScreen;
    
    [Header("Cooldown")]
    public float cooldown;
    [SerializeField] private string cooldownErrorMessage;
    [SerializeField] private Color cooldownErrorMessageColor;
    [SerializeField] private float cooldownErrorTimeOnScreen;
    private float _currentCooldown;
    
    
    [Header("Assignables")]
    public Player player;
    [SerializeField] private TrailRenderer[] trails;
    private Camera _cam;

    [Header("Parameters")]
    private float slidingScale = 0.5f;
    private float playerScale;
    public float slideForce = 400;
    public float maxSlideTime;
    public float ceilingDistanceCheck;
    public float wallDistanceCheck;
    public float checkWidth;
    public float maxSlopeAngle;
    private float slideTime;
    
    public bool sliding;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Start()
    {
        base.Start();
        playerScale =  transform.localScale.y;
    }
    private void Update() 
    {
        if (!mechanicUnlocked) return;
        
        if (player.input.Dashing && player.movement.grounded && !sliding)
        {
            if (_currentCooldown > 0)
            {
                //Still on cd
                EventManager.Instance.Trigger("OnSendUIMessageTemporary", 
                    cooldownErrorMessage, 
                    cooldownErrorMessageColor, 
                    cooldownErrorTimeOnScreen);

            }
            else
            {
                if (!player.energy.TakeEnergy(energyCost))
                {
                    EventManager.Instance.Trigger("OnSendUIMessageTemporary", 
                        energyErrorMessage, 
                        energyErrorMessageColor, 
                        energyErrorTimeOnScreen);
                    return;
                }
            
                EventManager.Instance.Trigger("OnPlayerDashCd", cooldown);
                
                StartSlide();
            }
            
        }
        
        _currentCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (sliding)
        {
            SlideMovement();
        }
    }

    private Vector3 slideDirection = Vector3.zero;
    private bool ableToControlOld;
    private void StartSlide()
    {
        player.hookshot.DestroyHook();
        
        sliding = true;
        player.movement.ableToControl = false;
        ableToControlOld = player.movement.ableToMove;

        foreach (TrailRenderer item in trails)
        {
            item.emitting = true;
        }
        
        StartCD();
        player.dash.StartCD();
        
        transform.localScale = new Vector3(transform.localScale.x, slidingScale, transform.localScale.z);
        player.movement.rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        
        Vector3 input = player.movement.playerModel.forward * player.input.Y +
                        player.movement.playerModel.right * player.input.X;
        
        slideDirection = input == Vector3.zero ? player.movement.playerModel.forward : input; 
        slideTime = maxSlideTime;
        
        StartCoroutine(CameraEffect(10, 40, 0.25f));
        
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerDash));
    }
    
    public void StartCD()
    {
        _currentCooldown = cooldown;
    }

    private void SlideMovement()
    {
        if (!CheckSlope() || player.movement.rb.velocity.y > -0.1f)
        {
            player.movement.rb.AddForce(slideDirection * slideForce, ForceMode.Force);
        
            slideTime -= Time.deltaTime;

            player.jump.SetAbleToJump(true);
        
            if (slideTime <= 0 || CheckFrontWall())
            {
                if (!CheckCeiling())
                {
                    StopSlide();
                }
                else
                {
                    player.jump.SetAbleToJump(false);
                }
            }
        }
        else
        {
            player.movement.rb.AddForce(GetSlopeMoveDirection(slideDirection) * slideForce, ForceMode.Force);
        }
    }

    private bool CheckCeiling()
    {
        Vector3 leftOrigin = player.transform.position - (player.transform.right * checkWidth);
        Vector3 rightOrigin = player.transform.position + (player.transform.right * checkWidth);

        bool left = Physics.Raycast(leftOrigin, player.transform.up, ceilingDistanceCheck, player.movement.groundMask);
        bool right = Physics.Raycast(rightOrigin, player.transform.up, ceilingDistanceCheck, player.movement.groundMask);
        
        return left || right;
    }

    private bool CheckFrontWall()
    {
        Vector3 leftOrigin = player.transform.position - (player.transform.right * checkWidth);
        Vector3 rightOrigin = player.transform.position + (player.transform.right * checkWidth);

        bool left = Physics.Raycast(leftOrigin, player.movement.rb.velocity.normalized.Flatten(1,0,1), wallDistanceCheck, player.movement.groundMask);
        bool right = Physics.Raycast(rightOrigin, player.movement.rb.velocity.normalized.Flatten(1,0,1), wallDistanceCheck, player.movement.groundMask);

        return left || right;
    }

    private RaycastHit slopeHit;
    private bool CheckSlope()
    {
        if (Physics.Raycast(player.transform.position, -player.transform.up, out slopeHit, 2f * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(player.transform.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public void StopSlide()
    {
        sliding = false;
        transform.localScale = new Vector3(transform.localScale.x, playerScale, transform.localScale.z);

        foreach (TrailRenderer item in trails)
        {
            item.emitting = false;
        }
        
        player.movement.ableToControl = ableToControlOld;
    }

    private IEnumerator CameraEffect(float power = 10, float steps = 10, float transitionPoint = 0.5f)
    {
        float speedTime = maxSlideTime * transitionPoint;
        float slowTime = maxSlideTime - speedTime;

        for (int i = 0;
             i < steps;
             i++)
        {
            _cam.fieldOfView += power / steps;
            yield return new WaitForSeconds(speedTime / steps);
        }

        for (int i = 0;
             i < steps;
             i++)
        {
            _cam.fieldOfView -= power / steps;
            yield return new WaitForSeconds(slowTime / steps);
        }
    }

    private void OnDrawGizmos()
    {
        if (sliding)
        {
            Vector3 leftOrigin = player.transform.position - (player.transform.right * checkWidth);
            Vector3 rightOrigin = player.transform.position + (player.transform.right * checkWidth);
            Debug.DrawLine(leftOrigin, leftOrigin + (player.transform.up * ceilingDistanceCheck));
            Debug.DrawLine(leftOrigin, leftOrigin + (player.movement.rb.velocity.normalized * wallDistanceCheck));
            
            Debug.DrawLine(rightOrigin, rightOrigin + (player.transform.up * ceilingDistanceCheck));
            Debug.DrawLine(rightOrigin, rightOrigin + (player.movement.rb.velocity.normalized * wallDistanceCheck));
        }
    }
}