using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILevelSystem : MonoBehaviour
{
    [System.Serializable]
    public struct UILevel
    {
        public string name;
        public Player.EXPType expType;
        [HideInInspector] public LevellingSystem levelSystem;
        public Utilities_RadialProgressBar progressBar;
        public TextMeshProUGUI level;
    }

    public List<UILevel> levelUIs;
    public Player player;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        for (int i = 0; i < levelUIs.Count; i++)
        {
            UILevel current = levelUIs[i];

            current.levelSystem = GetLevellingSystemOfType(current.expType);

            current.progressBar.SetRange(current.levelSystem.GetLevelStructure(current.levelSystem.Level).expRequirement, current.levelSystem.GetLevelStructure(current.levelSystem.Level + 1).expRequirement);
            current.progressBar.SetValue(current.levelSystem.EXP);
            current.level.text = "LV. " + current.levelSystem.Level;
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < levelUIs.Count; i++)
        {
            UILevel current = levelUIs[i];

            current.levelSystem = GetLevellingSystemOfType(current.expType);

            int EXP = current.levelSystem.EXP;
            if (EXP >= current.progressBar.MaxValue)
            {
                System.Action onFinish = delegate
                {
                    current.progressBar.SetRange(current.progressBar.MaxValue, current.levelSystem.GetLevelStructure(current.levelSystem.Level + 1).expRequirement);
                    current.progressBar.SetValue(EXP);
                    current.level.text = "LV. " + current.levelSystem.Level;
                };

                current.progressBar.SetValue(current.progressBar.MaxValue, onFinish);
            }
            else
            {
                current.progressBar.SetValue(EXP);
            }
        }
    }

    private LevellingSystem GetLevellingSystemOfType(Player.EXPType type)
    {
        switch (type)
        {
            case Player.EXPType.Attack:
                return player.attackLevelSystem;
            case Player.EXPType.Defense:
                return player.defenseLevelSystem;
            case Player.EXPType.Mobility:
                return player.mobilityLevelSystem;
        }

        throw new System.Exception("Levelling System not found");
    }
}