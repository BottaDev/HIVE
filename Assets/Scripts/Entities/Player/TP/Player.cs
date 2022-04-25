using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [Header("Assignable")] 
    public PlayerInput input;
    public PlayerAnimator animator;
    public PlayerJump jump;
    public PlayerMovement movement;
    public Shoot shoot;
    public Dash dash;
    public PlayerLevel level;
    public PlayerDebugDevTools debug;
    public PlayerAim aim;
    public PlayerGrappleV2 grapple;
    public PlayerGrenadeThrow grenadeThrow;

    [SerializeField] private UILevelSystem levelSystemUI;
    [SerializeField] private HealthBar healthBar;

    private bool Restart => input.Restart;
    public float MaxHP{get{ return maxHealth; } set { maxHealth = value; healthBar.SetMaxHealt(maxHealth);} }

    protected override void Awake()
    {
        base.Awake();

        EventManager.Instance?.Subscribe(EventManager.Events.OnPlayerDead, PlayerDeath);
        EventManager.Instance?.Subscribe(EventManager.Events.OnPlayerDamaged, OnPlayerDamaged);
    }

    private void Start()
    {
        healthBar.SetMaxHealt(maxHealth);
        healthBar.SetHealth(maxHealth);
    }

    private void Update()
    {
        if (Restart) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddExp(PlayerLevel.ExpType type, int amount)
    {
        level.AddExp(type, amount);

        levelSystemUI.UpdateUI(type);
    }

    private void OnPlayerDamaged(params object[] parameters)
    {
        TakeDamage((float) parameters[0]);
    }

    public override void TakeDamage(float damage)
    {
        if (debug.Invincible) return;
        
        CurrentHealth -= damage;
        EventManager.Instance.Trigger(EventManager.Events.OnLifeUpdated, CurrentHealth);

        if (!(CurrentHealth <= 0)) return;
        
        EventManager.Instance.Trigger(EventManager.Events.OnPlayerDead);
        EventManager.Instance.Unsubscribe(EventManager.Events.OnPlayerDamaged, OnPlayerDamaged);
    }

    public void PlayerDeath(params object[] parameters)
    {
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerDeath));
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        gameObject.SetActive(false);
        SceneManager.LoadScene(2);
    }
}