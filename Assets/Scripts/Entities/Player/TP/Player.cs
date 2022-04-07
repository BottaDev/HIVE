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
    public ShootTP shoot;
    public DashTP dash;

    public LevellingSystem attackLevelSystem;
    public LevellingSystem defenseLevelSystem;
    public LevellingSystem mobilityLevelSystem;
    public enum EXPType
    {
        Attack, Defense, Mobility
    }

    public bool restart { get { return input.restart; } }

    [SerializeField] private UILevelSystem _levelSystemUI;
    [SerializeField] private HealthBar _healthBar;
    
    protected override void Awake()
    {
        base.Awake();

        Func<int, int> levelFormula = delegate (int level) { return (level-1) * 20; };
        attackLevelSystem = new LevellingSystem(levelFormula).SetOnLevelup(delegate(int level) { shoot.damage += 1;});
        defenseLevelSystem = new LevellingSystem(levelFormula).SetOnLevelup(delegate (int level) { maxHealth += 2; });
        mobilityLevelSystem = new LevellingSystem(levelFormula).SetOnLevelup(delegate (int level) { movement.maxSpeed += 1; });

        EventManager.Instance?.Subscribe(EventManager.Events.OnPlayerDamaged, OnPlayerDamaged);
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


    public void AddEXP(EXPType type, int amount)
    {
        switch (type)
        {
            case EXPType.Attack:
                attackLevelSystem.EXP += amount;
                break;
            case EXPType.Defense:
                defenseLevelSystem.EXP += amount;
                break;
            case EXPType.Mobility:
                mobilityLevelSystem.EXP += amount;
                break;
        }

        _levelSystemUI.UpdateUI();
    }
    private void OnPlayerDamaged(params object[] parameters)
    {
        TakeDamage((float)parameters[0]);
    }

    public override void TakeDamage(float damage)
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
