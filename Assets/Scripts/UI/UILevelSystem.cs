using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UILevelSystem : MonoBehaviour
{
    [System.Serializable]
    public struct UILevel
    {
        public string name;
        public PlayerLevel.ExpType expType;
        public Utilities_RadialProgressBar progressBar;
    }

    public List<UILevel> levelUIs;
    public TextMeshProUGUI level;
    public TextMeshProUGUI expAmountText;

    private Player _player;

    private void Awake()
    {
        EventManager.Instance.Subscribe("SendPlayerReference", GetPlayerReference);
        EventManager.Instance.Subscribe("OnPlayerLevelSystemUpdate", UpdateUI);
    }

    private void Start()
    {
        EventManager.Instance.Trigger("NeedsPlayerReference");
    }

    private void GetPlayerReference(params object[] p)
    {
        _player = (Player)p[0];
        Initialize();
    }

    public void Initialize()
    {
        PlayerLevel level = _player.level;
        LevellingSystem system = level.system;

        for (int i = 0; i < levelUIs.Count; i++)
        {
            UILevel current = levelUIs[i];
            PlayerLevel.Exp exp = _player.level.FindExpOfType(current.expType);

            current.progressBar.SetRange(0, system.GetDifferenceBetweenLevels(system.Level, system.Level+1));
            current.progressBar.SetValue(exp.ThisLevel);
        }

        UpdateLevelText();
        UpdateEXPAmountText();
    }

    public void UpdateUI(params object[] p)
    {
        PlayerLevel.ExpType lastTypeGained = (PlayerLevel.ExpType) p[0];
        PlayerLevel level = _player.level;
        LevellingSystem system = level.system;
        
        
        float previousValue = 0;
        for (int i = 0; i < levelUIs.Count; i++)
        {
            UILevel current = levelUIs[i];
            PlayerLevel.Exp exp = level.FindExpOfType(current.expType);
            
            if (level.ThisLevel >= current.progressBar.MaxValue && exp.type == lastTypeGained)
            {
                System.Action onFinish = delegate
                {
                    int difference = level.ThisLevel - (int)current.progressBar.MaxValue;

                    float previousValue = 0;
                    for (int i = 0; i < levelUIs.Count; i++)
                    {
                        UILevel current = levelUIs[i];
                        PlayerLevel.Exp exp = level.FindExpOfType(current.expType);

                        if(level.DelayList.Count > 0)
                        {
                            List<int> list = level.DelayList.Where((x => x.Item1 == exp.type)).Select((x => x.Item2)).ToList();
                            int total = 0;

                            for (int j = 0; j < list.Count; j++)
                            {
                                total += list[j];
                            }
                            
                            exp.ThisLevel = total;
                        }
                        else
                        {
                            exp.ThisLevel = 0;
                        }

                        
                        int differenceBetweenLevels = system.GetDifferenceBetweenLevels(system.Level, system.Level + 1);
                        current.progressBar.SetRange(0, differenceBetweenLevels);
                        current.progressBar.SetValue(exp.ThisLevel + previousValue);
                        UpdateLevelText();
                        level.ThisLevel = difference;
                        previousValue += exp.ThisLevel;
                        UpdateEXPAmountText();
                    }

                    level.isDelayed = false;
                    level.DelayList.Clear();
                };

                level.isDelayed = true;
                current.progressBar.SetValue(exp.ThisLevel + previousValue, onFinish);
            }
            else
            {
                current.progressBar.SetValue(exp.ThisLevel + previousValue);
            }

            previousValue += exp.ThisLevel;
        }
        
        UpdateEXPAmountText();
    }

    private void UpdateEXPAmountText()
    {
        PlayerLevel level = _player.level;
        LevellingSystem system = level.system;
        int differenceBetweenLevels = system.GetDifferenceBetweenLevels(system.Level, system.Level + 1);
        expAmountText.text = $"{level.ThisLevel} / {differenceBetweenLevels}"; 
    }
    
    private void UpdateLevelText()
    {
        level.text = "LV. " + _player.level.system.Level;
    }
}