using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public enum ExpType
    {
        Attack, Defense, Mobility
    }

    [SerializeField] private Player player;

    public LevellingSystem system;
    public List<Exp> exp;
    public bool isDelayed;

    public readonly List<Tuple<ExpType, int>> DelayList = new List<Tuple<ExpType, int>>();
    private int _thisLevel;

    //List of upgrades that will happen in order (First entry will happen at 5, second at 10, etc)
    private Queue<Action> _upgradesEvery5Levels;

    public int Level => system.Level;
    public int ThisLevel { get => _thisLevel; set => _thisLevel = value; }

    private void Start()
    {
        InitializeUpgrades();
        InitializeLevels();
    }

    private void InitializeUpgrades()
    {
        _upgradesEvery5Levels = new Queue<Action>();

        //Level 5 = double jump
        _upgradesEvery5Levels.Enqueue(delegate { player.jump.amountOfJumps = 2; });

        //Level 10
    }

    private void InitializeLevels()
    {
        int LevelFormula(int level)
        {
            return (level - 1) * 20;
        }

        system = new LevellingSystem(LevelFormula).SetOnLevelup(OnLevelup).AddEveryXLevels(5, delegate
        {
            if (_upgradesEvery5Levels.Count > 0)
            {
                _upgradesEvery5Levels.Dequeue()();
            }
        });
    }

    public Exp FindExpOfType(ExpType type)
    {
        return exp.Find(x => x.type == type);
    }

    public void AddExp(ExpType type, int amount)
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

        if (currentLevel < system.Level)
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
    }

    private void OnLevelup(int level)
    {
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