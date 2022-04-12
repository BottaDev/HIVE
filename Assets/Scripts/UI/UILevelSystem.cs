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

    public Player player;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        PlayerLevel level = player.level;
        LevellingSystem system = level.system;

        for (int i = 0; i < levelUIs.Count; i++)
        {
            UILevel current = levelUIs[i];
            PlayerLevel.Exp exp = player.level.FindExpOfType(current.expType);

            current.progressBar.SetRange(0, system.GetDifferenceBetweenLevels(system.Level, system.Level+1));
            current.progressBar.SetValue(exp.ThisLevel);
        }

        this.level.text = "LV. " + system.Level;
    }

    public void UpdateUI(PlayerLevel.ExpType lastTypeGained)
    {
        PlayerLevel level = player.level;
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

                            Debug.Log(exp.type.ToString() + ": "+ total);
                            exp.ThisLevel = total;
                        }
                        else
                        {
                            exp.ThisLevel = 0;
                        }

                        
                        int differenceBetweenLevels = system.GetDifferenceBetweenLevels(system.Level, system.Level + 1);
                        current.progressBar.SetRange(0, differenceBetweenLevels);
                        current.progressBar.SetValue(exp.ThisLevel + previousValue);
                        this.level.text = "LV. " + system.Level;
                        level.ThisLevel = difference;
                        previousValue += exp.ThisLevel;
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
    }
}