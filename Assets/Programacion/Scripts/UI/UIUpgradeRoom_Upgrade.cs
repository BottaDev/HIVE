using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeRoom_Upgrade : MonoBehaviour
{
    public GameObject separationLine;
    public Image icon;
    public TextMeshProUGUI name;
    public TextMeshProUGUI description;
    public TextMeshProUGUI number;
    
    private PlayerUpgrades.Upgrade savedUpgrade;
    public void SetUpgrade(PlayerUpgrades.Upgrade upgrade, int index)
    {
        if (upgrade.icon == null)
        {
            icon.sprite = upgrade.icon;
            name.text = "None";
            description.text = "This is an empty upgrade, it will do literally nothing for you, so don't choose it.";
        }
        else
        {
            icon.sprite = upgrade.icon;
            name.text = upgrade.name.Colorize(PlayerUpgrades.GetColorOfType(upgrade.type));
            description.text = upgrade.longDescription;
        }

        number.text = $"{index + 1}";
    }

    public void ChooseUpgrade()
    {
        UILevelUpgradePrompt.instance.ChooseUpgrade(savedUpgrade);
    }
}
