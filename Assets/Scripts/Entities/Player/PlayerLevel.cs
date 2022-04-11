using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private Player player;
    public enum EXPType
    {
        Attack, Defense, Mobility
    }

    [System.Serializable]
    public class EXP
    {
        [System.Serializable]
        public class ResetEveryXLevels
        {
            public int resetEvery;
            private int _exp;
            public int exp { get { return _exp; } set { _exp = value; } }
        }

        public string name;
        public EXPType type;
        public List<ResetEveryXLevels> levelResets;

        private int _total;
        private int _thisLevel;

        public int total { get { return _total; } set { _total = value; } }
        public int thisLevel { get { return _thisLevel; } set { SetThisLevel(value); } }

        public void SetThisLevel(int newValue)
        {
            _thisLevel = newValue;
        }
    }

    public LevellingSystem system;
    public List<EXP> exp;

    public int level { get { return system.Level; } }
    private int _thisLevel;
    public int thisLevel { get { return _thisLevel; } set { _thisLevel = value; } }

    //List of upgrades that will happen in order (First entry will happen at 5, second at 10, etc)
    Queue<Action> upgradesEvery5Levels;

    private void Start()
    {
        InitializeUpgrades();
        InitializeLevels();
    }

    void InitializeUpgrades()
    {
        upgradesEvery5Levels = new Queue<Action>();

        //Level 5 = double jump
        upgradesEvery5Levels.Enqueue(delegate { player.jump.amountOfJumps = 2; });

        //Level 10
    }
    void InitializeLevels()
    {
        Func<int, int> levelFormula = delegate (int level) { return (level - 1) * 20; };
        system = new LevellingSystem(levelFormula)
            .SetOnLevelup(OnLevelup)
            .AddEveryXLevels(5, delegate { if(upgradesEvery5Levels.Count > 0) upgradesEvery5Levels.Dequeue()(); })
            ;
    }

    public EXP FindEXPOfType(EXPType type)
    {
        return exp.Find((x => x.type == type));
    }

    public List<Tuple<EXPType, int>> delayList = new List<Tuple<EXPType, int>>();
    public bool isDelayed;
    public void AddEXP(EXPType type, int amount)
    {
        if (isDelayed)
        {
            delayList.Add(new Tuple<EXPType, int>(type, amount));
        }
        
        EXP exp = FindEXPOfType(type);
        exp.total += amount;
        exp.thisLevel += amount;
        thisLevel += amount;

        List<EXP.ResetEveryXLevels> list = this.exp.Find((x => x.type == type)).levelResets;
        for (int i = 0; i < list.Count; i++)
        {
            EXP.ResetEveryXLevels reset = list[i];

            reset.exp += amount;
        }

        int currentLevel = system.Level;
        system.EXP += amount;

        if(currentLevel < system.Level)
        {
            //levelup
            int difference = system.Level - currentLevel;
            for (int i = 0; i < difference; i++)
            {
                int levelUp = currentLevel + i + 1;

                for (int j = 0; j < this.exp.Count; j++)
                {
                    EXP cur = this.exp[j];

                    for (int t = 0; t < cur.levelResets.Count; t++)
                    {
                        EXP.ResetEveryXLevels reset = cur.levelResets[t];

                        if (levelUp % reset.resetEvery == 0)
                        {
                            if(cur.type == type)
                            {
                                int differenceBetweenLevels = system.GetDifferenceBetweenLevels(levelUp - reset.resetEvery, levelUp);
                                for (int f = 0; f < this.exp.Count; f++)
                                {
                                    EXP c = this.exp[f];
                                    int res = c.levelResets.Find(x => x.resetEvery == reset.resetEvery).exp;
                                    differenceBetweenLevels -= res;
                                }

                                reset.exp = -differenceBetweenLevels;


                                List<EXP> test = this.exp.Where((x => x.type != type)).ToList();
                                for (int f = 0; f < test.Count; f++)
                                {
                                    EXP c = test[f];
                                    c.levelResets.Find(x => x.resetEvery == reset.resetEvery).exp = 0;
                                }
                            }
                        }
                    }
                }
                
            }
        }
    }

    void OnLevelup(int level)
    {
        
    }
}
