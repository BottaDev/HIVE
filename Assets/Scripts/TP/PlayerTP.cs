using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTP : Entity
{
    private float _playerHeight = 2;

    [Header("Movement")] 
    public float moveSpeed = 6;
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
    public Camera cam;
    
    //[Header("Levels")]
    //public float experience = 0;
    //public float totalExperience = 3;
    //public GameObject firsPassive;
    //public GameObject secondPassive;
    //public GameObject thirdPassive;
    //public GameObject upgrade;
    //private bool _canUpgrade;

    public GameObject crosshair;
    private HealthBar _healthBar;

    private void Awake()
    {
        base.Awake();

        EventManager.Instance.Subscribe("OnPlayerDamaged", OnPlayerDamaged);
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        //_healthBar = FindObjectOfType<HealthBar>();
        //if (_healthBar == null)
        //    Debug.LogError("Health Bar could not been found!");
        //else
        //    _healthBar.SetMaxHealt(maxHealth);

        //upgrade.SetActive(false);
        //firsPassive.SetActive(false);
        //secondPassive.SetActive(false);
        //thirdPassive.SetActive(false);
        //crosshair.SetActive(true);
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask);
        
        MyInput();
        ControlDrag();

        if (Input.GetKeyDown(jumpKey) && _isGrounded)
            Jump();

        //if (Input.GetKeyDown(KeyCode.H) && _canUpgrade)
        //{
        //    _canUpgrade = false;
        //    experience = 0;
        //    OpenUpgrades();
        //}
            
        _slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, _slopeHit.normal);

        //ExperienceCount();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        transform.forward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
    }

    private void MyInput()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = transform.forward * _verticalMovement + transform.right * _horizontalMovement;
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
            _rb.velocity = new Vector3(moveDirection.normalized.x * moveSpeed, _rb.velocity.y, moveDirection.normalized.z * moveSpeed);
        else if (_isGrounded && OnSlope())
            _rb.velocity = new Vector3(_slopeMoveDirection.normalized.x * moveSpeed, _rb.velocity.y, moveDirection.normalized.z * moveSpeed);
        else if (!_isGrounded)
            _rb.velocity = new Vector3(moveDirection.normalized.x * moveSpeed * airMultiplier, _rb.velocity.y, moveDirection.normalized.z * moveSpeed * airMultiplier);
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

    //private void ExperienceCount()
    //{
    //    if (experience >= totalExperience)
    //    {
    //        _canUpgrade = true;
    //        upgrade.SetActive(true);
    //    }
    //}

    //private void OpenUpgrades()
    //{
    //    Cursor.visible = true;
    //    Cursor.lockState = CursorLockMode.Confined;

    //    crosshair.SetActive(false);
    //    upgrade.SetActive(false);
    //    firsPassive.SetActive(true);
    //    secondPassive.SetActive(true);
    //    thirdPassive.SetActive(true);
    //}

    //public void Button()
    //{
    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Locked;

    //    crosshair.SetActive(true);
    //    upgrade.SetActive(false);
    //    firsPassive.SetActive(false);
    //    secondPassive.SetActive(false);
    //    thirdPassive.SetActive(false);
    //}

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
