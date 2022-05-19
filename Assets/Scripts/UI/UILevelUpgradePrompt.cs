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
    [SerializeField] private List<UIUpgrade> uiUpgrades;
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

    private Queue<ChoosableUpgradePrompt> choices = new Queue<ChoosableUpgradePrompt>();
    private ChoosableUpgradePrompt current;

    public List<PlayerUpgrades.Upgrade> upgradesChosen = new List<PlayerUpgrades.Upgrade>();

    
    public enum EyeState
    {
        closed, open, ultra
    }

    [System.Serializable]
    public struct UIUpgrade
    {
        public string name;
        public Image background;
        public Image icon;
    }
    class ChoosableUpgradePrompt
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
        EventManager.Instance.Subscribe("OnPlayerEnteredUpgradeRoom", ShowLongDescription);
        EventManager.Instance.Subscribe("OnPlayerLeftUpgradeRoom", HideLongDescription);
        EventManager.Instance.Trigger("NeedsPlayerReference");
        ShowUI(false);
        shortText.gameObject.SetActive(true);
        longText.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (waitingForInput)
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ChooseUpgrade(0);
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ChooseUpgrade(1);
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ChooseUpgrade(2);
                }

                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ChooseUpgrade(3);
                }
            }
        }
    }

    private void GetPlayerReference(params object[] p)
    {
        _player = (Player)p[0];
    }
    
    void ChooseUpgrade(int index)
    {
        PlayerUpgrades.Upgrade upgrade = current.upgrades.SafeGet(index).Item1;
        upgradesChosen.Add(upgrade);
        upgrade.action.Invoke(_player);
        waitingForInput = false;
        ShowUI(false);
        choiceAmount--;

        if (choices.Count > 0)
        {
            NextUpgrade();
        }
        
        CheckEyeState();
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
        
        /*
        string prefix = "";
        if(choiceAmount > 1)
        {
            prefix = $"({choiceAmount - 1} Extra Upgrade Choices Left)\n";
        }

        shortText.text = prefix + current.displayText;
        longText.text = prefix + current.longDisplayText;*/

        for (int i = 0; i < current.upgrades.Count; i++)
        {
            PlayerUpgrades.Upgrade upgrade = current.upgrades[i].Item1;
            Sprite icon = upgrade.icon;
            Sprite background = current.upgrades[i].Item2;

            UIUpgrade ui = uiUpgrades[i];
            if (upgrade.name == null)
            {
                ui.background.gameObject.SetActive(false);
                ui.icon.gameObject.SetActive(false);
            }
            else
            {
                ui.background.gameObject.SetActive(true);
                ui.icon.gameObject.SetActive(true);
                
                ui.background.sprite = background;
                ui.icon.sprite = icon;
            }
            
        }
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
