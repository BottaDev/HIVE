using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public static class GameStats
{
    public static float leftGunDMG;
    public static float rightGunDMG;
    
    public static int enemiesKilled;
    public static float movementSPD;

    public static int hpLost;
    public static float energyUsed;
    
    public static List<PlayerUpgrades.Upgrade> upgrades = new List<PlayerUpgrades.Upgrade>();
}
public class UIExtraInfoScreen : MonoBehaviour
{
    public static UIExtraInfoScreen i;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI rerunText;
    
    public TextMeshProUGUI upgradesText;

    public TextMeshProUGUI statsText;

    public Utilities_CanvasGroupReveal reveal;
    private bool showing;
    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        EventManager.Instance.Subscribe("ExtraInfoScreenInput", OnTriggerInput);
    }

    void OnTriggerInput(params object[] obj)
    {
        reveal.RevealToggle();
    }
    public void SetLevel(string levelName)
    {
        levelText.text = levelName;
    }

    public void SetRerun(int rerunCount)
    {
        rerunText.text = "Rerun number: " + rerunCount;
    }

    public void SetUpgrades(List<PlayerUpgrades.Upgrade> upgrades)
    {
        upgradesText.text = "";

        if (upgrades.Count == 0) return;
        
        //IA2-P1
        List<PlayerUpgrades.Upgrade> orderedUpgrades = upgrades.OrderByDescending(x => x.rank).ToList();
        List<PlayerUpgrades.Upgrade> bigUpgrades = orderedUpgrades.TakeWhile(x => x.rank >= 3).ToList();
        List<PlayerUpgrades.Upgrade> smallUpgrades = orderedUpgrades.SkipWhile(x => x.rank >= 3).ToList();

        bool first;
        if (bigUpgrades.Count != 0)
        {
            first = true;
            upgradesText.text += "\n";
            upgradesText.text += "BIG UPGRADES: \n";
            foreach (var upgrade in bigUpgrades)
            {
                if (!first)
                {
                    upgradesText.text += "\n";
                }

                upgradesText.text += $"-{upgrade.name}".Colorize(PlayerUpgrades.GetColorOfType(upgrade.type));
                first = false;
            }
        }

        if (smallUpgrades.Count != 0)
        {
            first = true;
            upgradesText.text += "\n";
            upgradesText.text += "\n";
            upgradesText.text += "SMALL UPGRADES: \n";
            foreach (var upgrade in smallUpgrades)
            {
                if (!first)
                {
                    upgradesText.text += "\n";
                }

                upgradesText.text += $"-{upgrade.name}".Colorize(PlayerUpgrades.GetColorOfType(upgrade.type));
                first = false;
            }
        }
    }

    public void UpdateStats()
    {
        string stats = $"Left Gun DMG: {GameStats.leftGunDMG}\n\n";

        stats += $"Right Gun DMG: {GameStats.rightGunDMG}\n\n";
        stats += $"Enemies Killed: {GameStats.enemiesKilled}\n\n";
        stats += $"Movement SPD: {GameStats.movementSPD}\n\n";
        stats += $"HP Lost (all time): {GameStats.hpLost}\n\n";
        stats += $"Energy Used (all time): {GameStats.energyUsed}";

        statsText.text = stats;
    }
}
