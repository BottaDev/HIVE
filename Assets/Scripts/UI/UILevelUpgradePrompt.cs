using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.UI;

public class UILevelUpgradePrompt : MonoBehaviour
{
    public static UILevelUpgradePrompt instance;

    [Header("Assignables")]
    [SerializeField] private GameObject showObject;
    [SerializeField] private List<UISlot> uiUpgrades;
    [SerializeField] private TextMeshProUGUI shortText;
    [SerializeField] private TextMeshProUGUI longText;

    [Header("Upgrade Room Error Message")]
    [SerializeField] private string errorMessage;
    [SerializeField] private Color errorMessageColor;


    [Header("Colors")]
    [SerializeField] private Sprite attackColor;
    [SerializeField] private Sprite defenseColor;
    [SerializeField] private Sprite mobilityColor;

    [Header("Eye State")]
    public GameObject closed;
    public GameObject open;
    public GameObject ultra;
    private EyeState currentEyeState;
    
    
    private bool waitingForInput;

    public Queue<ChoosableUpgradePrompt> choices = new Queue<ChoosableUpgradePrompt>();
    public ChoosableUpgradePrompt current;

    public List<PlayerUpgrades.Upgrade> upgradesChosen = new List<PlayerUpgrades.Upgrade>();

    
    public enum EyeState
    {
        closed, open, ultra
    }
    
    public class ChoosableUpgradePrompt
    {
        public string displayText;
        public string longDisplayText;
        public List<Tuple<PlayerUpgrades.Upgrade, Sprite>> upgrades;
        public PlayerUpgrades.UpgradeType type;
    }

    private int choiceAmount;
    private Player _player;
    
    private void Awake()
    {
        instance = this;
        EventManager.Instance.Subscribe("SendPlayerReference", GetPlayerReference);
    }
    private void Start()
    {
        //EventManager.Instance.Subscribe("OnPlayerEnteredUpgradeRoom", ShowLongDescription);
        //EventManager.Instance.Subscribe("OnPlayerLeftUpgradeRoom", HideLongDescription);
        EventManager.Instance.Trigger("NeedsPlayerReference");
        ShowUI(false);
        shortText.gameObject.SetActive(true);
        longText.gameObject.SetActive(false);

        UIExtraInfoScreen.i.SetUpgrades(GameStats.upgrades);
        
        if (Player.SavedPlayer != null)
        {
            if (Player.SavedPlayer.currentUpgrade != null)
            {
                choices.Enqueue(Player.SavedPlayer.currentUpgrade);
            }
            
            foreach (var choice in Player.SavedPlayer.choices)
            {
                choices.Enqueue(choice);
            }

            if (choices.Count > 0)
            {
                NextUpgrade();
            }
        }
    }
    
    private void Update()
    {
        if (waitingForInput)
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ChooseUpgrade(current.upgrades.SafeGet(0).Item1);
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ChooseUpgrade(current.upgrades.SafeGet(1).Item1);
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ChooseUpgrade(current.upgrades.SafeGet(2).Item1);
                }

                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ChooseUpgrade(current.upgrades.SafeGet(3).Item1);
                }
            }
        }
    }

    private void GetPlayerReference(params object[] p)
    {
        _player = (Player)p[0];
    }
    
    public void ChooseUpgrade(PlayerUpgrades.Upgrade upgrade)
    {
        upgradesChosen.Add(upgrade);
        GameStats.upgrades.Add(upgrade);
        UIExtraInfoScreen.i.SetUpgrades(GameStats.upgrades);
        upgrade.action.Invoke(_player);
        waitingForInput = false;
        ShowUI(false);
        choiceAmount--;

        if (choices.Count > 0)
        {
            NextUpgrade();
        }
        else
        {
            current = null;
            UIUpgradeRoom_UpgradeList.i.Done();
        }
        
        CheckEyeState();
        
        _player.SavePlayer();
    }
    public void SetUpgrades(List<PlayerUpgrades.Upgrade> upgrades, PlayerUpgrades.UpgradeType type)
    {
        
        string shortResult = $"{type.ToString().ToUpper()} LEVEL UPGRADE: \n";
        string longResult = $"{type.ToString().ToUpper()} LEVEL UPGRADE: \n(Full description)\n";
        
        for (int i = 0; i < upgrades.Count; i++)
        {
            PlayerUpgrades.Upgrade current = upgrades[i];
            string addTextShort = $"\n{i+1} - {current.name}: {current.description}";
            //shortResult += addTextShort.Colorize(GetColorOfType(current.type));
            
            string addTextLong = $"\n{i+1} - {current.name}: {current.longDescription}";
            //longResult += addTextLong.Colorize(GetColorOfType(current.type));
        }

        List<Tuple<PlayerUpgrades.Upgrade, Sprite>> tuples = new List<Tuple<PlayerUpgrades.Upgrade, Sprite>>();
        
        foreach (var upgrade in upgrades)
        {
            tuples.Add(Tuple.Create(upgrade, GetBackgroundType(upgrade.type)));
        }
        
        ChoosableUpgradePrompt add = new ChoosableUpgradePrompt(){displayText = shortResult, upgrades = tuples, longDisplayText = longResult, type = type};

        bool firstChoice = choices.Count == 0;
        choices.Enqueue(add);
        choiceAmount++;
        
        
        
        
        if (firstChoice && !waitingForInput)
        {
            NextUpgrade();
        }
    }

    private void NextUpgrade()
    {
        current = choices.Dequeue();
        waitingForInput = true;
        ShowUI(true);
        CheckEyeState();

        for (int i = 0; i < current.upgrades.Count; i++)
        {
            PlayerUpgrades.Upgrade upgrade = current.upgrades[i].Item1;
            Sprite icon = upgrade.icon;
            Sprite background = current.upgrades[i].Item2;

            UISlot ui = uiUpgrades[i];
            ui.SetBackground(background);
            ui.SetIcon(icon);
        }
        
        //IA2-P1
        UIUpgradeRoom_UpgradeList.i?.SetUpgrades(current.upgrades.Select(x=> x.Item1).ToList());
    }

    public void ShowUI(bool state)
    {
        showObject.SetActive(state);

        if (state)
        {
            EventManager.Instance.Trigger("OnEliminateUIMessage", errorMessage);
        }
    }

    public void ShowLongDescription(params object[] p)
    {
        LongDescription(true);
        
        if (!showObject.activeSelf)
        {
            EventManager.Instance.Trigger("OnSendUIMessage", errorMessage, errorMessageColor);
        }

    }
    public void HideLongDescription(params object[] p)
    {
        LongDescription(false);
        
        if (!showObject.activeSelf)
        {
            EventManager.Instance.Trigger("OnEliminateUIMessage", errorMessage);
        }
    }
    void LongDescription(bool state)
    {
        if (state)
        {
            longText.gameObject.SetActive(true);
            shortText.gameObject.SetActive(false);
        }
        else
        {
            longText.gameObject.SetActive(false);
            shortText.gameObject.SetActive(true);
        }
    }

    void CheckEyeState()
    {
        if (choices.Count > 0 || waitingForInput)
        {
            bool ultraCondition = current.type == PlayerUpgrades.UpgradeType.Big;

            if (ultraCondition)
            {
                SetEyeState(EyeState.ultra);
            }
            else
            {
                SetEyeState(EyeState.open);
            }
        }
        else
        {
            SetEyeState(EyeState.closed);
        }
    }
    void SetEyeState(EyeState state)
    {
        currentEyeState = state;
        
        switch (state)
        {
            case EyeState.closed:
                closed.SetActive(true);
                open.SetActive(false);
                ultra.SetActive(false);
                break;
            
            case EyeState.open:
                closed.SetActive(false);
                open.SetActive(true);
                ultra.SetActive(false);
                break;
            
            case EyeState.ultra:
                closed.SetActive(false);
                open.SetActive(false);
                ultra.SetActive(true);
                break;
        }
    }

    private Sprite GetBackgroundType(PlayerLevel.ExpType type)
    {
        switch (type)
        {
            case PlayerLevel.ExpType.Attack:
                return attackColor;
            case PlayerLevel.ExpType.Defense:
                return defenseColor;
            case PlayerLevel.ExpType.Mobility:
                return mobilityColor;
                
            default:
                return attackColor;
        }
    }
}
