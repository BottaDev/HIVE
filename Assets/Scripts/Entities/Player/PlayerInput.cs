using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public enum MouseCode
{
    Left = 0,
    Right = 1,
    Middle = 2
}

public class PlayerInput : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] private MouseCode shootKeyLeft = MouseCode.Left;
    [SerializeField] private MouseCode shootKeyRight = MouseCode.Right;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode restartKey = KeyCode.R;
    [SerializeField] private KeyCode freeCamKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode grappleKey = KeyCode.Q;
    [SerializeField] private KeyCode directGrappleKey = KeyCode.C;
    [SerializeField] private KeyCode grenadeKey = KeyCode.E;
    [SerializeField] private KeyCode absorbKey = KeyCode.F;
    [SerializeField] private MouseCode railAttachKey = MouseCode.Right;
    [SerializeField] private KeyCode extraInfoKey = KeyCode.Tab;
    private bool _attaching;
    private bool _dashing;
    private bool _freecam;
    private bool _isMoving;
    private bool _jumping;
    private bool _restart;
    private bool _startedShootingLeft;
    private bool _shootingLeft;
    private bool _startedShootingRight;
    private bool _shootingRight;
    private bool _stoppedJumping;
    private bool _grapple;
    private bool _stoppedGrapple;
    private bool _directGrapple;
    private bool _stoppedDirectGrapple;
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
    public bool StartedShootingLeft { get => _startedShootingLeft; set => _startedShootingLeft = value; }
    public bool ShootingLeft { get => _shootingLeft; set => _shootingLeft = value; }
    public bool StartedShootingRight { get => _startedShootingRight; set => _startedShootingRight = value; }
    public bool ShootingRight { get => _shootingRight; set => _shootingRight = value; }
    public bool Restart { get => _restart; set => _restart = value; }
    public bool Freecam { get => _freecam; set => _freecam = value; }
    public bool IsMoving { get => _isMoving; set => _isMoving = value; }
    public bool Attaching { get => _attaching; set => _attaching = value; }
    public bool Grapple { get => _grapple; set => _grapple = value; }
    public bool StoppedGrapple { get => _stoppedGrapple; set => _stoppedGrapple = value; }
    public bool DirectGrapple { get => _directGrapple; set => _directGrapple = value; }
    public bool StoppedDirectGrapple { get => _stoppedDirectGrapple; set => _stoppedDirectGrapple = value; }
    public bool GrenadeThrow { get => _grenadeThrow; set => _grenadeThrow = value; }
    public bool Absorbing { get => _absorbing; set => _absorbing = value; }
    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (UIPauseMenu.paused)
        {
            X = Input.GetAxisRaw("Horizontal");
            Y = Input.GetAxisRaw("Vertical");
            IsMoving = false;
            Jumping = false;
            StoppedJumping = false;
            Dashing = false;
            StartedShootingLeft = false;
            ShootingLeft = false;
            StartedShootingRight = false;
            ShootingRight = false;
            Restart = false;
            Freecam = false;
            Attaching = false;
            Grapple = false;
            StoppedGrapple = false;
            GrenadeThrow = false;
            Absorbing = false;
            DirectGrapple = false;
            StoppedDirectGrapple = false;
        }
        else
        {
            X = Input.GetAxisRaw("Horizontal");
            Y = Input.GetAxisRaw("Vertical");
            IsMoving = X != 0 || Y != 0;
            Jumping = Input.GetKeyDown(jumpKey);
            StoppedJumping = Input.GetKeyUp(jumpKey);
            Dashing = Input.GetKeyDown(dashKey);
            StartedShootingLeft = Input.GetMouseButtonDown((int) shootKeyLeft);
            ShootingLeft = Input.GetMouseButton((int) shootKeyLeft);
            StartedShootingRight = Input.GetMouseButtonDown((int) shootKeyRight);
            ShootingRight = Input.GetMouseButton((int) shootKeyRight);
            Restart = Input.GetKeyDown(restartKey);
            Freecam = Input.GetKey(freeCamKey);
            Attaching = Input.GetMouseButtonDown((int) railAttachKey);
            Grapple = Input.GetKeyDown(grappleKey);
            StoppedGrapple = Input.GetKeyUp(grappleKey);
            GrenadeThrow = Input.GetKeyDown(grenadeKey);
            Absorbing = Input.GetKey(absorbKey);
            DirectGrapple = Input.GetKeyDown(directGrappleKey);
            StoppedDirectGrapple= Input.GetKeyUp(directGrappleKey);

            if (Input.GetKeyDown(extraInfoKey))
            {
                EventManager.Instance.Trigger("ExtraInfoScreenInput");
            }
        }
        
        
    }
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
//[CustomEditor(typeof(PlayerInput))]
//This editor is not necessary, because its just a bunch of enums. Its just here for future's sake.
public class KamCustomEditor_PlayerInput : KamCustomEditor
{
    private PlayerInput editorTarget;
    private void OnEnable()
    {
        editorTarget = (PlayerInput)target;
    }
    
    public override void GameDesignerInspector()
    {
        EditorGUILayout.LabelField("Keybinds", EditorStyles.centeredGreyMiniLabel);
        
    }
}
#endif
#endregion