using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTP : Entity
{
    [Header("Movement")] 
    public float moveSpeed = 6;
    public float airMultiplier = 0.4f;
    private float _horizontalMovement;
    private float _verticalMovement;
    
    [Header("Jumping")] 
    public float jumpForce = 15;
    private KeyCode jumpKey = KeyCode.Space;
    
    [Header("Ground Detection")]
    public LayerMask groundMask;
    private bool _isGrounded;
    private float _groundDistance = 0.1f;
    private Transform _groundCheck;

    [Header("Slope")]
    private RaycastHit _slopeHit;
    private Vector3 _slopeMoveDirection;
    
    [Header("Drag")]
    private float _groundDrag = 6;
    private float _airDrag = 1;
    
    private float _playerHeight = 2;
    
    private Vector3 _moveDirection;
    
    private Rigidbody _rb;
    private Camera _cam;
    private HealthBar _healthBar;

    private void Awake()
    {
        base.Awake();

        //EventManager.Instance.Subscribe("OnPlayerDamaged", OnPlayerDamaged);
    }

    private void Start()
    {
        _cam = Camera.main;
        _groundCheck = GameObject.Find("Ground Check").GetComponent<Transform>();
        
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _healthBar = FindObjectOfType<HealthBar>();
        if (_healthBar == null)
            Debug.LogError("Health Bar could not been found!");
        else
            _healthBar.SetMaxHealt(maxHealth);
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, groundMask);
        
        MyInput();
        ControlDrag();

        if (Input.GetKeyDown(jumpKey) && _isGrounded)
            Jump();

        _slopeMoveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal);
    }

    private void FixedUpdate()
    {
        MovePlayer();
        transform.forward = new Vector3(_cam.transform.forward.x, 0, _cam.transform.forward.z);
    }

    private void MyInput()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");

        _moveDirection = transform.forward * _verticalMovement + transform.right * _horizontalMovement;
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
        if (_isGrounded && !OnSlope())
            _rb.velocity = new Vector3(_moveDirection.normalized.x * moveSpeed, _rb.velocity.y, _moveDirection.normalized.z * moveSpeed);
        else if (_isGrounded && OnSlope())
            _rb.velocity = new Vector3(_slopeMoveDirection.normalized.x * moveSpeed, _rb.velocity.y, _moveDirection.normalized.z * moveSpeed);
        else if (!_isGrounded)
            _rb.velocity = new Vector3(_moveDirection.normalized.x * moveSpeed * airMultiplier, _rb.velocity.y, _moveDirection.normalized.z * moveSpeed * airMultiplier);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight / 2 + 0.5f))
        {
            if (_slopeHit.normal != Vector3.up)
                return true;
        }
        return false;
    }

    public void OnPlayerDamaged(params object[] parameters)
    {
        TakeDamage((float)parameters[0]);
    }

    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth);

        if (CurrentHealth <= 0)
            this.gameObject.SetActive(false);
    }
}
