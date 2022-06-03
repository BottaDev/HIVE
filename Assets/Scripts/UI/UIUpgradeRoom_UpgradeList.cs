using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeRoom_UpgradeList : ObjectList
{
    public static UIUpgradeRoom_UpgradeList i;

    private void Awake()
    {
        i = this;
    }

    public void SetUpgrades(List<PlayerUpgrades.Upgrade> upgrades)
    {
        ClearList();
        bool first = true;
        for (int i = 0; i < upgrades.Count; i++)
        {
            GameObject obj = AddObject();
            UIUpgradeRoom_Upgrade script = obj.GetComponent<UIUpgradeRoom_Upgrade>();
            script.SetUpgrade(upgrades[i],i);
            if (first)
            {
                script.separationLine.SetActive(false);
                first = false;
            }
        }
    }

    public void Done()
    {
        ClearList();
        EventManager.Instance.Trigger("OnUpgradeRoomCanvasEnd");
    }
}
