using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UILevelUpgradePrompt : MonoBehaviour
{
    public static UILevelUpgradePrompt instance;

    [Header("Assignables")]
    [SerializeField] private GameObject showObject;
    [SerializeField] private TextMeshProUGUI shortText;
    [SerializeField] private TextMeshProUGUI longText;

    [Header("Upgrade Room Error Message")]
    [SerializeField] private string errorMessage;
    [SerializeField] private Color errorMessageColor;


    [Header("Colors")]
    [SerializeField] private Color attackColor;
    [SerializeField] private Color defenseColor;
    [SerializeField] private Color mobilityColor;

    private bool waitingForInput;

    private Queue<ChoosableUpgradePrompt> choices = new Queue<ChoosableUpgradePrompt>();
    private ChoosableUpgradePrompt current;
    class ChoosableUpgradePrompt
    {
        public string displayText;
        public string longDisplayText;
        public List<PlayerUpgrades.Upgrade> upgrades;
    }

    private int choiceAmount;

    private void Awake()
    {
        instance = this;
        
    }
    private void Start()
    {
        EventManager.Instance.Subscribe(EventManager.Events.OnPlayerEnteredUpgradeRoom, ShowLongDescription);
        EventManager.Instance.Subscribe(EventManager.Events.OnPlayerLeftUpgradeRoom, HideLongDescription);
        
        ShowUI(false);
        shortText.gameObject.SetActive(true);
        longText.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (choices.Count > 0)
        {
            if (!waitingForInput)
            {
                current = choices.Dequeue();
                waitingForInput = true;

                ShowUI(true);
            }

            string prefix = "";
            if(choiceAmount > 1)
            {
                prefix = $"({choiceAmount - 1} Extra Upgrade Choices Left)\n";
            }

            shortText.text = prefix + current.displayText;
            longText.text = prefix + current.longDisplayText;
        }

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

    void ChooseUpgrade(int index)
    {
        current.upgrades.SafeGet(index).action.Invoke();
        waitingForInput = false;
        ShowUI(false);
        choiceAmount--;
    }
    public void SetUpgrades(List<PlayerUpgrades.Upgrade> upgrades, PlayerUpgrades.UpgradeType type)
    {
        string shortResult = $"{type.ToString().ToUpper()} LEVEL UPGRADE: \n";
        string longResult = $"{type.ToString().ToUpper()} LEVEL UPGRADE: \n(Full description)\n";
        
        for (int i = 0; i < upgrades.Count; i++)
        {
            PlayerUpgrades.Upgrade current = upgrades[i];
            string addTextShort = $"\n{i+1} - {current.name}: {current.description}";
            shortResult += addTextShort.Colorize(GetColorOfType(current.type));
            
            string addTextLong = $"\n{i+1} - {current.name}: {current.longDescription}";
            longResult += addTextLong.Colorize(GetColorOfType(current.type));
        }

        ChoosableUpgradePrompt add = new ChoosableUpgradePrompt(){displayText = shortResult, upgrades = upgrades, longDisplayText = longResult};

        choices.Enqueue(add);
        choiceAmount++;
    }

    private Color GetColorOfType(PlayerLevel.ExpType type)
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

    public void ShowUI(bool state)
    {
        showObject.SetActive(state);
    }

    public void ShowLongDescription(params object[] p)
    {
        LongDescription(true);
        
        if (!showObject.activeSelf)
        {
            EventManager.Instance.Trigger(EventManager.Events.OnSendUIMessage, errorMessage, errorMessageColor);
        }

    }
    public void HideLongDescription(params object[] p)
    {
        LongDescription(false);
        
        if (!showObject.activeSelf)
        {
            EventManager.Instance.Trigger(EventManager.Events.OnEliminateUIMessage, errorMessage);
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
}
