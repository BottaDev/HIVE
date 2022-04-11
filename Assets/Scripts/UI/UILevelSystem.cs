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
        public PlayerLevel.EXPType expType;
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
            PlayerLevel.EXP exp = player.level.FindEXPOfType(current.expType);

            current.progressBar.SetRange(0, system.GetDifferenceBetweenLevels(system.Level, system.Level+1));
            current.progressBar.SetValue(exp.thisLevel);
        }

        this.level.text = "LV. " + system.Level;
    }

    public void UpdateUI(PlayerLevel.EXPType lastTypeGained)
    {
        PlayerLevel level = player.level;
        LevellingSystem system = level.system;

        float previousValue = 0;
        for (int i = 0; i < levelUIs.Count; i++)
        {
            UILevel current = levelUIs[i];
            PlayerLevel.EXP exp = level.FindEXPOfType(current.expType);
            
            if (level.thisLevel >= current.progressBar.MaxValue && exp.type == lastTypeGained)
            {
                System.Action onFinish = delegate
                {
                    int difference = level.thisLevel - (int)current.progressBar.MaxValue;

                    float previousValue = 0;
                    for (int i = 0; i < levelUIs.Count; i++)
                    {
                        UILevel current = levelUIs[i];
                        PlayerLevel.EXP exp = level.FindEXPOfType(current.expType);

                        if(level.delayList.Count > 0)
                        {
                            List<int> list = level.delayList.Where((x => x.Item1 == exp.type)).Select((x => x.Item2)).ToList();
                            int total = 0;

                            for (int j = 0; j < list.Count; j++)
                            {
                                total += list[j];
                            }

                            Debug.Log(exp.type.ToString() + ": "+ total);
                            exp.thisLevel = total;
                        }
                        else
                        {
                            exp.thisLevel = 0;
                        }

                        
                        int differenceBetweenLevels = system.GetDifferenceBetweenLevels(system.Level, system.Level + 1);
                        current.progressBar.SetRange(0, differenceBetweenLevels);
                        current.progressBar.SetValue(exp.thisLevel + previousValue);
                        this.level.text = "LV. " + system.Level;
                        level.thisLevel = difference;
                        previousValue += exp.thisLevel;
                    }

                    level.isDelayed = false;
                    level.delayList.Clear();
                };

                level.isDelayed = true;
                current.progressBar.SetValue(exp.thisLevel + previousValue, onFinish);
            }
            else
            {
                current.progressBar.SetValue(exp.thisLevel + previousValue);
            }

            previousValue += exp.thisLevel;
        }
    }
}