using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UILevelUpgradePrompt : MonoBehaviour
{
    public static UILevelUpgradePrompt instance;

    public TextMeshProUGUI text;

    public Color attackColor;
    public Color defenseColor;
    public Color mobilityColor;
    
    private bool waitingForInput;

    private Queue<ChoosableUpgradePrompt> choices = new Queue<ChoosableUpgradePrompt>();
    private ChoosableUpgradePrompt current;
    class ChoosableUpgradePrompt
    {
        public string displayText;
        public List<PlayerUpgrades.Upgrade> upgrades;
    }

    private int choiceAmount;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        text.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (choices.Count > 0)
        {
            if (!waitingForInput)
            {
                current = choices.Dequeue();
                waitingForInput = true;
                text.gameObject.SetActive(true);
            }

            if(choiceAmount > 1)
            {
                text.text = $"({choiceAmount - 1} Extra Upgrade Choices Left)\n" + current.displayText;
            }
            else
            {
                text.text = current.displayText;
            }
            
        }

        if (waitingForInput)
        {
            if (!Input.GetKey(KeyCode.LeftControl))
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
        text.gameObject.SetActive(false);
        choiceAmount--;
    }
    public void SetUpgrades(List<PlayerUpgrades.Upgrade> upgrades, PlayerUpgrades.UpgradeType type)
    {
        string result = $"{type.ToString().ToUpper()} LEVEL UPGRADE: \n";

        for (int i = 0; i < upgrades.Count; i++)
        {
            PlayerUpgrades.Upgrade current = upgrades[i];
            string addText = $"\n{i+1} - {current.name}: {current.description}";
            result += addText.Colorize(GetColorOfType(current.type));
        }

        ChoosableUpgradePrompt add = new ChoosableUpgradePrompt(){displayText = result, upgrades = upgrades};

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
}
