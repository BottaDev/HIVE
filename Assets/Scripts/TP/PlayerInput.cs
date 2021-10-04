using UnityEngine;

public class PlayerInput : MonoBehaviour
{    
    [Header("Jump")]
    private KeyCode _jumpKey = KeyCode.Space;

    private Player _player;

    protected void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        ProcessInput();
        
        if (Input.GetKeyDown(_jumpKey) && _player.isGrounded)
            _player.Jump();
    }

    private void ProcessInput()
    {
        _player.horizontalMovement = Input.GetAxisRaw("Horizontal");
        _player.verticalMovement = Input.GetAxisRaw("Vertical");

        _player.moveDirection = transform.forward * _player.verticalMovement + transform.right * _player.horizontalMovement;
    }
}
