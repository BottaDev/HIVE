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

        player.movement.ApplyImpulseForce(input.normalized * slideForce, maxSlideTime);
        slideDirection = player.movement.playerModel.forward;
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
        player.movement.rb.AddForce(slideDirection * slideForce, ForceMode.Force);

        slideTime -= Time.deltaTime;

        if (slideTime <= 0)
        {
            StopSlide();
        }
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
}