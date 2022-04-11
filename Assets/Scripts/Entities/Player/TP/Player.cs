using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Player : Entity
{
    [Header("Assignables")]
    public PlayerInput input;
    public PlayerAnimator animator;
    public PlayerJump jump;
    public PlayerMovement movement;
    public Shoot shoot;
    public Dash dash;
    public PlayerLevel level;
    public PlayerDebugDevTools debug;

    public bool restart { get { return input.restart; } }

    [SerializeField] private UILevelSystem _levelSystemUI;
    [SerializeField] private HealthBar _healthBar;
    

    protected override void Awake()
    {
        base.Awake();

        EventManager.Instance?.Subscribe(EventManager.Events.OnPlayerDamaged, OnPlayerDamaged);
        EventManager.Instance?.Subscribe(EventManager.Events.OnEnemyDamaged, Debug_EnemyDamaged);
    }

    private void Start()
    {
        _healthBar.SetMaxHealt(maxHealth);
    }

    private void Update()
    {
        if (restart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Debug_EnemyDamaged(params object[] obj)
    {
        Debug.Log("Enemy damaged");
        Crosshair.instance.Hit();
    }
    public void AddEXP(PlayerLevel.EXPType type, int amount)
    {
        level.AddEXP(type, amount);

        _levelSystemUI.UpdateUI(type);
    }
    private void OnPlayerDamaged(params object[] parameters)
    {
        TakeDamage((float)parameters[0]);
    }

    public override void TakeDamage(float damage)
    {
        if (!debug.invincible)
        {
            CurrentHealth -= damage;
            EventManager.Instance.Trigger(EventManager.Events.OnLifeUpdated, CurrentHealth);

            if (CurrentHealth <= 0)
            {
                EventManager.Instance.Trigger(EventManager.Events.OnPlayerDead);
                EventManager.Instance.Unsubscribe(EventManager.Events.OnPlayerDamaged, OnPlayerDamaged);
                gameObject.SetActive(false);
            }
        }
    }
}
