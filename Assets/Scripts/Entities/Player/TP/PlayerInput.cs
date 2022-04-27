using UnityEngine;

public enum MouseCode
{
    Left = 0,
    Right = 1,
    Middle = 2
}

public class PlayerInput : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] private MouseCode shootKey = MouseCode.Left;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode restartKey = KeyCode.R;
    [SerializeField] private KeyCode freeCamKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode grappleKey = KeyCode.Q;
    [SerializeField] private KeyCode grenadeKey = KeyCode.E;
    [SerializeField] private KeyCode absorbKey = KeyCode.F;
    [SerializeField] private MouseCode railAttachKey = MouseCode.Right;
    private bool _attaching;
    private bool _dashing;
    private bool _freecam;
    private bool _isMoving;
    private bool _jumping;
    private bool _restart;
    private bool _shooting;
    private bool _stoppedJumping;
    private bool _grapple;
    private bool _stoppedGrapple;
    private bool _grenadeThrow;
    private bool _absorbing;
    //Input variables (Used by other scripts to run their actions)
    private float _x;
    private float _y;

    public float X { get => _x; set => _x = value; }
    public float Y { get => _y; set => _y = value; }
    public bool Jumping { get => _jumping; set => _jumping = value; }
    public bool StoppedJumping { get => _stoppedJumping; set => _stoppedJumping = value; }
    public bool Dashing { get => _dashing; set => _dashing = value; }
    public bool Shooting { get => _shooting; set => _shooting = value; }
    public bool Restart { get => _restart; set => _restart = value; }
    public bool Freecam { get => _freecam; set => _freecam = value; }
    public bool IsMoving { get => _isMoving; set => _isMoving = value; }
    public bool Attaching { get => _attaching; set => _attaching = value; }
    public bool Grapple { get => _grapple; set => _grapple = value; }
    public bool StoppedGrapple { get => _stoppedGrapple; set => _stoppedGrapple = value; }
    public bool GrenadeThrow { get => _grenadeThrow; set => _grenadeThrow = value; }
    public bool Absorbing { get => _absorbing; set => _absorbing = value; }
    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        X = Input.GetAxisRaw("Horizontal");
        Y = Input.GetAxisRaw("Vertical");
        IsMoving = X != 0 || Y != 0;
        Jumping = Input.GetKeyDown(jumpKey);
        StoppedJumping = Input.GetKeyUp(jumpKey);
        Dashing = Input.GetKeyDown(dashKey);
        Shooting = Input.GetMouseButton((int) shootKey);
        Restart = Input.GetKeyDown(restartKey);
        Freecam = Input.GetKey(freeCamKey);
        Attaching = Input.GetMouseButtonDown((int) railAttachKey);
        Grapple = Input.GetKeyDown(grappleKey);
        StoppedGrapple = Input.GetKeyUp(grappleKey);
        GrenadeThrow = Input.GetKeyDown(grenadeKey);
        Absorbing = Input.GetKey(absorbKey);
    }
}