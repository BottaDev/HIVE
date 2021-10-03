using UnityEngine;

public class Player : Entity
{
    [Header("Movement")] 
    public float moveSpeed;
    public float airMultiplier;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public Vector3 moveDirection;

    [Header("Ground Detection")] 
    public LayerMask groundMask;
    [HideInInspector] public bool isGrounded;
    private float _groundDistance = 0.1f;
    private Transform _groundCheck;
    
    [Header("Jump")] 
    [SerializeField] private float _jumpForce = 30f;

    [Header("Slope")] 
    private RaycastHit _slopeHit;
    private Vector3 _slopeMoveDirection;

    [Header("Drag")] 
    private float _groundDrag;
    private float _airDrag;

    private float _playerHeight;

    private Rigidbody _rb;
    private Camera _cam;
    private HealthBar _healthBar;
    
    protected override void Awake()
    {
        base.Awake();

        _rb = GetComponent<Rigidbody>();
        _groundCheck = GameObject.Find("Ground Check").GetComponent<Transform>();
        
        EventManager.Instance.Subscribe("OnPlayerDamaged", OnPlayerDamaged);
    }

    private void Start()
    {
        _cam = Camera.main;
        
        _rb.freezeRotation = true;

        _healthBar = FindObjectOfType<HealthBar>();
        if(_healthBar == null)
            Debug.LogError("Health Bar could not been found!");
        else
            _healthBar.SetMaxHealt(maxHealth);
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, groundMask);

        ControlDrag();
        
        _slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, _slopeHit.normal);
    }

    private void FixedUpdate()
    {
        MovePlayer();
        
        transform.forward = new Vector3(_cam.transform.forward.x, 0, _cam.transform.forward.z);
    }

    private void ControlDrag()
    {
        if (isGrounded)
            _rb.drag = _groundDrag;
        else
            _rb.drag = _airDrag;
    }

    private void MovePlayer()
    {
        if (isGrounded && !OnSlope())
            _rb.velocity = new Vector3(moveDirection.normalized.x * moveSpeed, _rb.velocity.y, moveDirection.normalized.z * moveSpeed);
        else if (isGrounded && OnSlope())
            _rb.velocity = new Vector3(_slopeMoveDirection.normalized.x * moveSpeed, _rb.velocity.y, _slopeMoveDirection.normalized.z * moveSpeed);
        else if (!isGrounded)
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

    public void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void OnPlayerDamaged(params object[] parameters)
    {
        TakeDamage((float)parameters[0]);
    }

    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth);

        if (CurrentHealth <= 0)
        {
            EventManager.Instance.Trigger("OnPlayerDead");
            EventManager.Instance.Unsubscribe("OnPlayerDamaged", OnPlayerDamaged);
            gameObject.SetActive(false);
        }
    }
}
