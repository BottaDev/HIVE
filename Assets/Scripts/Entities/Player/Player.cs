using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [Header("Assignable")]
    public PlayerEnergy energy;
    public PlayerInput input;
    public PlayerAnimator animator;
    public PlayerJump jump;
    public PlayerMovement movement;
    public Shoot shoot;
    public Dash dash;
    public PlayerLevel level;
    public PlayerDebugDevTools debug;
    public PlayerAim aim;
    public PlayerDirectHookshot hookshot;
    public PlayerGrenadeThrow grenadeThrow;


    public Rails attachedRail { get; set; }
    public bool attachedToRail => attachedRail != null;
    public static Progress SavedPlayer;

    public class Progress
    {
        //HP stuff
        public int currentHP;
        
        //Energy stuff
        public float currentEnergy;
        
        //Level stuff
        public int level;
        public List<PlayerUpgrades.Upgrade> upgrades = new List<PlayerUpgrades.Upgrade>();
    }
    
    private bool Restart => input.Restart;
    public int MaxHP
    {
        get => maxHealth;
        set
        {
            int difference =  value - maxHealth;
            CurrentHealth += difference;
            maxHealth = value; 
            EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth, maxHealth);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        EventManager.Instance?.Subscribe("OnPlayerDead", PlayerDeath);
        EventManager.Instance?.Subscribe("OnPlayerDamaged", OnPlayerDamaged);
        EventManager.Instance?.Subscribe("NeedsPlayerReference", SendPlayerReference);
    }
    public void SendPlayerReference(params object[] p)
    {
        EventManager.Instance.Trigger("SendPlayerReference", this);
    }

    private void Start()
    {
        if (SavedPlayer != null)
        {
            //Load everything
            //First, do all upgrades you had.
            foreach (var upgrade in SavedPlayer.upgrades)
            {
                upgrade.action.Invoke(this);
            }
            
            
            //Then set your current hp and energy.
            CurrentHealth = SavedPlayer.currentHP;
            energy.Current = SavedPlayer.currentEnergy;
        }
        
        EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth, MaxHP);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Restart)
            {
                SavePlayer();
                RestartScene();
            }
        }
        else if (Restart)
        {
            DeleteSavedPlayer();
            RestartScene();
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DeleteSavedPlayer()
    {
        SavedPlayer = null;
    }
    public void SavePlayer()
    {
        if (SavedPlayer == null)
        {
            SavedPlayer = new Progress()
            {
                currentEnergy = energy.Current, 
                currentHP = CurrentHealth,
                level = level.system.Level,
                upgrades = UILevelUpgradePrompt.instance.upgradesChosen
            };
        }
        else
        {
            SavedPlayer.currentEnergy = energy.Current;
            SavedPlayer.currentHP = CurrentHealth;
            SavedPlayer.level = level.system.Level;

            foreach (var upgrade in UILevelUpgradePrompt.instance.upgradesChosen)
            {
                SavedPlayer.upgrades.Add(upgrade);
            }
            
        }
    }

    public void AddExp(PlayerLevel.ExpType type, int amount)
    {
        level.AddExp(type, amount);
    }

    private void OnPlayerDamaged(params object[] parameters)
    {
        TakeDamage((int) parameters[0]);
    }

    public override void TakeDamage(int damage)
    {
        if (debug.Invincible) return;
        
        CurrentHealth -= damage;
        EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth, MaxHP);

        if (!(CurrentHealth <= 0)) return;
        
        DeleteSavedPlayer();
        EventManager.Instance.Trigger("OnPlayerDead");
        EventManager.Instance.Unsubscribe("OnPlayerDamaged", OnPlayerDamaged);
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