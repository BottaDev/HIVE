using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _playerHeight = 2;

    [SerializeField] private Transform orientation;
    
    [Header("Movement")] 
    public float moveSpeed = 6;
    public float movementMultiplier = 10;
    [SerializeField] private float airMultiplier = 0.4f;
    
    [Header("Jumping")] 
    public float jumpForce = 15;
    
    [Header("Keybinds")] 
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    
    [Header("Drag")]
    private float _groundDrag = 6;
    private float _airDrag = 1;
    

    private float _horizontalMovement;
    private float _verticalMovement;

    [Header("Ground Detection")] 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    private bool _isGrounded;
    private float _groundDistance = 0.4f;

    public Vector3 moveDirection;
    private Vector3 _slopeMoveDirection;

    private Rigidbody _rb;

    private RaycastHit _slopeHit;

    [Header("Levels")]
    public float experience = 0;
    public float totalExperience = 3;
    public GameObject firsPassive;
    public GameObject secondPassive;
    public GameObject thirdPassive;
    public GameObject upgrade;
    private bool _canUpgrade;

    public GameObject crosshair;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        
        upgrade.SetActive(false);
        firsPassive.SetActive(false);
        secondPassive.SetActive(false);
        thirdPassive.SetActive(false);
        crosshair.SetActive(true);
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask);
        
        MyInput();
        ControlDrag();

        if (Input.GetKeyDown(jumpKey) && _isGrounded)
            Jump();

        if (Input.GetKeyDown(KeyCode.H) && _canUpgrade)
        {
            _canUpgrade = false;
            experience = 0;
            OpenUpgrades();
        }
            
        _slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, _slopeHit.normal);

        ExperienceCount();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * _verticalMovement + orientation.right * _horizontalMovement;
    }

    private void ControlDrag()
    {
        if (_isGrounded)
            _rb.drag = _groundDrag;
        else
            _rb.drag = _airDrag;
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void MovePlayer()
    {
        if(_isGrounded && !OnSlope())
            _rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        else if(_isGrounded && OnSlope())
            _rb.AddForce(_slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        else if(!_isGrounded)
            _rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight / 2 + 0.5f))
        {
            if (_slopeHit.normal != Vector3.up)
                return true;
            else
                return false;
        }
        return false;
    }
    
    private void ExperienceCount()
    {
        if (experience >= totalExperience)
        {
            _canUpgrade = true;
            upgrade.SetActive(true);
        }
    }

    private void OpenUpgrades()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        crosshair.SetActive(false);
        upgrade.SetActive(false);
        firsPassive.SetActive(true);
        secondPassive.SetActive(true);
        thirdPassive.SetActive(true);
    }

    public void Button()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        crosshair.SetActive(true);
        upgrade.SetActive(false);
        firsPassive.SetActive(false);
        secondPassive.SetActive(false);
        thirdPassive.SetActive(false);
    }
}