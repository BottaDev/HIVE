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
        attackLevelSystem = new LevellingSystem(levelFormula);
        defenseLevelSystem = new LevellingSystem(levelFormula);
        mobilityLevelSystem = new LevellingSystem(levelFormula);

        EventManager.Instance?.Subscribe("OnPlayerDamaged", OnPlayerDamaged);
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddEXP(EXPType.Attack, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddEXP(EXPType.Mobility, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddEXP(EXPType.Defense, 1);
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
        EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth);

        if (CurrentHealth <= 0)
        {
            EventManager.Instance.Trigger("OnPlayerDead");
            EventManager.Instance.Unsubscribe("OnPlayerDamaged", OnPlayerDamaged);
            gameObject.SetActive(false);
        }
    }
}
