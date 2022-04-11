using UnityEngine;
public enum MouseCode
{
    Left = 0,
    Right = 1,
    Middle = 2
}
public class PlayerInput : MonoBehaviour
{    
    [Header("Assignables")]
    [SerializeField] private Player _player;

    [Header("Keybinds")]
    [SerializeField] private MouseCode  _shootKey   = MouseCode.Left;
    [SerializeField] private KeyCode    _jumpKey    = KeyCode.Space;
    [SerializeField] private KeyCode    _dashKey    = KeyCode.LeftShift;
    [SerializeField] private KeyCode    _restartKey = KeyCode.R;
    [SerializeField] private KeyCode    _freeCamKey = KeyCode.LeftControl;
    [SerializeField] private MouseCode  _railAttachKey = MouseCode.Left;

    //Input variables (Used by other scripts to run their actions)
    float  _x;
    float  _y;
    bool   _jumping;
    bool   _stoppedJumping;
    bool   _dashing;
    bool   _shooting;
    bool   _restart;
    bool   _freecam;
    bool   _isMoving;
    bool   _attaching;

    public float x { get { return _x; } set { _x = value; } }
    public float y { get { return _y; } set { _y = value; } }
    public bool jumping { get { return _jumping; } set { _jumping = value; } }
    public bool stoppedJumping { get { return _stoppedJumping; } set { _stoppedJumping = value; } }
    public bool dashing { get { return _dashing; } set { _dashing = value; } }
    public bool shooting { get { return _shooting; } set { _shooting = value; } }
    public bool restart { get { return _restart; } set { _restart = value; } }
    public bool freecam { get { return _freecam; } set { _freecam = value; } }
    public bool isMoving { get { return _isMoving; } set { _isMoving = value; } }
    public bool attaching { get { return _attaching; } set { _attaching = value; } }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        isMoving = x != 0 || y != 0;
        jumping = Input.GetKeyDown(_jumpKey);
        stoppedJumping = Input.GetKeyUp(_jumpKey);
        dashing = Input.GetKeyDown(_dashKey);
        shooting = Input.GetMouseButton((int)_shootKey);
        restart = Input.GetKeyDown(_restartKey);
        freecam = Input.GetKey(_freeCamKey);
        attaching = Input.GetMouseButtonDown((int)_railAttachKey);
    }
}
