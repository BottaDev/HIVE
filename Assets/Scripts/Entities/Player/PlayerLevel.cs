using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class PlayerLevel : MonoBehaviour
{
    public enum ExpType
    {
        Attack, Defense, Mobility
    }

    [SerializeField] private Player player;

    public LevellingSystem system;
    public List<Exp> exp;
    public PlayerUpgrades upgrades;
    public bool isDelayed;

    public readonly List<Tuple<ExpType, int>> DelayList = new List<Tuple<ExpType, int>>();
    private int _thisLevel;

    public int Level => system.Level;
    public int ThisLevel { get => _thisLevel; set => _thisLevel = value; }

    private void Awake()
    {
        InitializeLevels();
    }

    private void InitializeLevels()
    {
        int LevelFormula(int level)
        {
            return (level - 1) * 20;
        }

        system = new LevellingSystem(LevelFormula).SetOnLevelup(OnLevelup)
            .AddEveryXLevels(3, OnBigLevelUp);
            
        system.SetExp(Player.SavedPlayer == null ? 0 : Player.SavedPlayer.totalExp, false);
    }

    public Exp FindExpOfType(ExpType type)
    {
        return exp.Find(x => x.type == type);
    }

    public void AddExp(ExpType type, int amount, bool doLevelUp = true)
    {
        if (isDelayed)
        {
            DelayList.Add(new Tuple<ExpType, int>(type, amount));
        }

        Exp exp = FindExpOfType(type);
        exp.Total += amount;
        exp.ThisLevel += amount;
        ThisLevel += amount;

        List<Exp.ResetEveryXLevels> list = this.exp.Find(x => x.type == type).levelResets;
        foreach (Exp.ResetEveryXLevels reset in list)
        {
            reset.Exp += amount;
        }

        int currentLevel = system.Level;
        system.Exp += amount;

        if (currentLevel < system.Level && doLevelUp)
        {
            //levelup
            int difference = system.Level - currentLevel;
            for (int i = 0; i < difference; i++)
            {
                int levelUp = currentLevel + i + 1;

                foreach (Exp cur in this.exp)
                {
                    foreach (Exp.ResetEveryXLevels reset in cur.levelResets)
                    {
                        if (levelUp % reset.resetEvery == 0)
                        {
                            if (cur.type == type)
                            {
                                int differenceBetweenLevels =
                                    system.GetDifferenceBetweenLevels(levelUp - reset.resetEvery, levelUp);
                                differenceBetweenLevels =
                                    (from c in this.exp
                                        let reset1 = reset
                                        select c.levelResets.Find(x => x.resetEvery == reset1.resetEvery).Exp)
                                    .Aggregate(differenceBetweenLevels, (current, res) => current - res);

                                reset.Exp = -differenceBetweenLevels;


                                List<Exp> test = this.exp.Where(x => x.type != type).ToList();
                                foreach (Exp c in test)
                                {
                                    c.levelResets.Find(x => x.resetEvery == reset.resetEvery).Exp = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        
        EventManager.Instance.Trigger("OnPlayerLevelSystemUpdate", type);
    }

    private void OnBigLevelUp(int level)
    {
        PlayerUpgrades.UpgradeType type = PlayerUpgrades.UpgradeType.Big;

        UILevelUpgradePrompt.instance.SetUpgrades(upgrades.GachaPull(type, exp), type);
    }
    private void OnLevelup(int level)
    {
        PlayerUpgrades.UpgradeType type = PlayerUpgrades.UpgradeType.Small;

        UILevelUpgradePrompt.instance.SetUpgrades(upgrades.GachaPull(type, exp), type);
        
        player.view.LevelUpEffect();
        player.view.Blink(1f,30f, Color.white);
        Popup.Create(player.model.transform.position, "LVL UP!",Color.white, new Vector2(0,5f)).ChangeSize(0.1f).SetParent(player.model.transform);
    }

    public List<Tuple<ExpType, int, int>> SaveToTupleList()
    {
        List<Tuple<ExpType, int, int>> list = new List<Tuple<ExpType, int, int>>();
        foreach (var expType in exp)
        {
            list.Add(new Tuple<ExpType, int, int>(expType.type, expType.ThisLevel, expType.Total));
        }

        return list;
    }
    [Serializable]
    public class Exp
    {
        public string name;
        public ExpType type;
        public List<ResetEveryXLevels> levelResets;
        private int _thisLevel;

        private int _total;

        public int Total { get => _total; set => _total = value; }
        public int ThisLevel { get => _thisLevel; set => SetThisLevel(value); }

        public void SetThisLevel(int newValue)
        {
            _thisLevel = newValue;
        }

        [Serializable]
        public class ResetEveryXLevels
        {
            public int resetEvery;
            private int _exp;
            public int Exp { get => _exp; set => _exp = value; }
        }
    }
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(PlayerLevel))]
public class KamCustomEditor_PlayerLevel : KamCustomEditor
{
    
}
#endif
#endregion