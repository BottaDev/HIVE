using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class Player : Entity
{
    [Header("Assignable")]
    public PlayerEnergy energy;
    public PlayerInput input;
    public PlayerJump jump;
    public PlayerMovement movement;
    public Shoot shoot;
    public Dash dash;
    public PlayerLevel level;
    public PlayerDebugDevTools debug;
    public PlayerDirectHookshot hookshot;
    public PlayerGrenadeThrow grenadeThrow;
    public PlayerView view;
    public Slide slide;
    public GameObject model;

    public Rails attachedRail { get; set; }
    public bool attachedToRail => attachedRail != null;
    public static Progress SavedPlayer;
    public static int rerunCount;
    public class Progress
    {
        //HP stuff
        public int currentHP;
        
        //Energy stuff
        public float currentEnergy;
        
        //Level stuff
        public int totalExp;
        public int expThisLevel;
        public List<PlayerLevel.Exp> exps = new List<PlayerLevel.Exp>();
        public List<PlayerUpgrades.Upgrade> upgrades = new List<PlayerUpgrades.Upgrade>();
        public Queue<UILevelUpgradePrompt.ChoosableUpgradePrompt> choices = new Queue<UILevelUpgradePrompt.ChoosableUpgradePrompt>();
        public UILevelUpgradePrompt.ChoosableUpgradePrompt currentUpgrade;
        
        //Gun stuff
        public PlayerGunStorage.Guns leftGun;
        public PlayerGunStorage.Guns rightGun;
    }
    
    private bool Restart => input.Restart;
    public int MaxHP
    {
        get => maxHealth;
        set
        {
            int difference =  value - maxHealth;
            Heal(difference);
            maxHealth = value; 
            EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth, maxHealth);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        EventManager.Instance.Subscribe("OnPlayerDead", PlayerDeath);
        EventManager.Instance.Subscribe("OnPlayerDamaged", OnPlayerDamaged);
        EventManager.Instance.Subscribe("NeedsPlayerReference", SendPlayerReference);
        EventManager.Instance.Subscribe("OnSaveGame", SavePlayer);
    }
    public void SendPlayerReference(params object[] p)
    {
        EventManager.Instance.Trigger("SendPlayerReference", this);
    }

    private void Start()
    {
        if (SavedPlayer != null)
        {
            rerunCount++;
            //Load everything
            foreach (var upgrade in SavedPlayer.upgrades)
            {
                upgrade.action.Invoke(this);
            }
            
            level.ThisLevel = SavedPlayer.expThisLevel;
            foreach (var exp in SavedPlayer.exps)
            {
                PlayerLevel.Exp current = level.exp.First(x => x.type == exp.type);
                current.Total = exp.Total;
                current.ThisLevel = exp.ThisLevel;
                EventManager.Instance.Trigger("OnPlayerLevelSystemUpdate", current.type);
            }

            DelayAction(0f,
                delegate {
                    EventManager.Instance.Trigger("OnPlayerLevelSystemUpdate", PlayerLevel.ExpType.Defense); 
                });
            
            CurrentHealth = SavedPlayer.currentHP;
            energy.Current = SavedPlayer.currentEnergy;
        }

        UIExtraInfoScreen.i.SetRerun(rerunCount);
        UIExtraInfoScreen.i.SetLevel(SceneManager.GetActiveScene().name);
        EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth, MaxHP);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
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
        SceneLoaderManager.instance.ReloadScene(SceneLoaderManager.SceneLoadType.normal);
    }

    public void DeleteSavedPlayer()
    {
        rerunCount = 0;
        GameStats.upgrades.Clear();
        SavedPlayer = null;
    }
    public void SavePlayer(params object[] obj)
    {
        if (SavedPlayer == null)
        {
            SavedPlayer = new Progress()
            {
                currentEnergy = energy.Current, 
                currentHP = CurrentHealth,
                totalExp = level.system.Exp,
                expThisLevel = level.ThisLevel,
                exps = level.exp,
                upgrades = UILevelUpgradePrompt.instance.upgradesChosen,
                choices = UILevelUpgradePrompt.instance.choices,
                currentUpgrade = UILevelUpgradePrompt.instance.current,
                leftGun = shoot.currentLeftGun,
                rightGun = shoot.currentRightGun
            };
        }
        else
        {
            SavedPlayer.currentEnergy = energy.Current;
            SavedPlayer.currentHP = CurrentHealth;

            foreach (var upgrade in UILevelUpgradePrompt.instance.upgradesChosen)
            {
                SavedPlayer.upgrades.Add(upgrade);
            }

            SavedPlayer.leftGun = shoot.currentLeftGun;
            SavedPlayer.rightGun = shoot.currentRightGun;
            SavedPlayer.exps = level.exp;
            SavedPlayer.totalExp = level.system.Exp;
            SavedPlayer.expThisLevel = level.ThisLevel;
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
        
        view.Blink(1f, 30f, Color.red);
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerHit));
        CurrentHealth -= damage;
        GameStats.hpLost += damage;
        UIExtraInfoScreen.i.UpdateStats();
        
        EventManager.Instance.Trigger("OnPlayerHit");
        EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth, MaxHP);

        if (CurrentHealth.ToPercentageOfRange(0, MaxHP) <= 20)
        {
            EventManager.Instance.Trigger("OnPlayerLowHealth");
        }
        
        if (!(CurrentHealth <= 0)) return;
        
        DeleteSavedPlayer();
        EventManager.Instance.Trigger("OnPlayerDead");
        EventManager.Instance.Unsubscribe("OnPlayerDamaged", OnPlayerDamaged);
    }

    public void Heal(int healAmount)
    {
        CurrentHealth += healAmount;
        
        if (CurrentHealth.ToPercentageOfRange(0, MaxHP) > 20)
        {
            EventManager.Instance.Trigger("OnPlayerRecoverFromLowHealth");
        }
        
        view.Blink(1f, 10f, Color.green);
        view.HealEffect();
        Popup.Create(model.transform.position, $"+{healAmount} HP",Color.green, new Vector2(0,3f)).ChangeSize(0.05f).SetParent(model.transform);
        EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth, MaxHP);
    }

    public void PlayerDeath(params object[] parameters)
    {
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(SFXs.PlayerDeath));
        
        gameObject.SetActive(false);
        SceneLoaderManager.instance.LoadScene(SceneLoaderManager.Scenes.LoseScreen);
    }


    public void DelayAction(float time, Action action)
    {
        StartCoroutine(Delay(time, action));
    }

    IEnumerator Delay(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }
    
   
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(Player))]
public class KamCustomEditor_Player : KamCustomEditor
{
    private Player editorTarget;
    private void OnEnable()
    {
        editorTarget = (Player)target;
    }

    public override void GameDesignerInspector()
    {
        EditorGUILayout.LabelField("Entity Parameters", EditorStyles.centeredGreyMiniLabel);

        editorTarget.maxHealth = EditorGUILayout.IntField(
            new GUIContent(
                "Max HP",
                "Max player health points."),
            editorTarget.maxHealth);
    }
}
#endif
#endregion