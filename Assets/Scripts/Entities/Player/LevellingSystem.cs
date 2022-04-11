using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class LevellingSystem
{
    #region Basic methods and Variables
    public struct LevelStructure
    {
        public int level;
        public int expRequirement;
    }
    public struct EveryXLevels
    {
        public int levelTrigger;
        public Action<int> action;
    }

    List<LevelStructure> levels;
    int maxLevel = 200;
    Func<int, int> LevelFormula = delegate(int level) { return level * 10; };
    Action<int> onLevelup = delegate(int level) { };
    List<EveryXLevels> levelTriggers = new List<EveryXLevels>();

    public void BuildBaseLevelSystem()
    {
        levels = new List<LevelStructure>();
        int lastExpRequirement = 0;

        for (int level = 1; level <= maxLevel; level++)
        {
            LevelStructure currentLevel = new LevelStructure();
            currentLevel.level = level;
            currentLevel.expRequirement = lastExpRequirement + LevelFormula(level);
            lastExpRequirement = currentLevel.expRequirement;
            levels.Add(currentLevel);
        }
    }
    public int GetCurrentLevel(int exp)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            LevelStructure currentLevel = levels[i];

            if (exp < currentLevel.expRequirement)
            {
                return levels[i - 1].level;
            }
        }

        return -1;
    }
    public int GetDifferenceBetweenLevels(int level1, int level2)
    {
        int lowerLevel = Mathf.Min(level1, level2);
        int higherLevel = Mathf.Max(level1, level2);

        int difference = GetLevelStructure(higherLevel).expRequirement - GetLevelStructure(lowerLevel).expRequirement;

        return difference;
    }
    public int GetDifferenceBetweenEXPandLevel(int exp, int level)
    {
        return GetLevelStructure(level).expRequirement - exp;
    }
    public LevelStructure GetLevelStructure(int level)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].level == level)
            {
                return levels[i];
            }
        }

        Debug.Log("Couldn't find the level of value" + level);
        return new LevelStructure();
    }
    #endregion

    public LevellingSystem(Func<int,int> levelFormula)
    {
        LevelFormula = levelFormula;
        BuildBaseLevelSystem();
        EXP = 0;
    }
    public LevellingSystem(Func<int, int> levelFormula, int EXP)
    {
        LevelFormula = levelFormula;
        BuildBaseLevelSystem();
        this.EXP = EXP;
    }
    public LevellingSystem SetMaxLevel(int newMaxLevel)
    {
        maxLevel = newMaxLevel;
        BuildBaseLevelSystem();
        return this;
    }
    public LevellingSystem SetStartingLevel(int level)
    {
        _level = level;
        EXP = GetLevelStructure(level).expRequirement;
        return this;
    }
    public LevellingSystem SetOnLevelup(Action<int> onLevelup)
    {
        this.onLevelup = onLevelup;
        return this;
    }
    public LevellingSystem AddEveryXLevels(int everyXLevels, Action<int> action)
    {
        levelTriggers.Add(new EveryXLevels { levelTrigger = everyXLevels, action = action });
        return this;
    }

    int _level;
    int _exp;

    public int Level { get { UpdateLevelSystem(); return _level; } private set { _level = value; } }
    public int EXP { get { return _exp; } set { _exp = value; UpdateLevelSystem(); } }

    public void UpdateLevelSystem()
    {
        int updatedLevel = GetCurrentLevel(_exp);

        if (updatedLevel == -1)
        {
            Debug.Log("Could not find level of exp amount " + _exp);
            return;
        }

        if(_level < updatedLevel)
        {
            int difference = updatedLevel - _level;
            for (int i = 0; i < difference; i++)
            {
                int levelUp = _level + i + 1;
                onLevelup(levelUp);

                for (int j = 0; j < levelTriggers.Count; j++)
                {
                    if(levelUp % levelTriggers[j].levelTrigger == 0)
                    {
                        levelTriggers[j].action(levelUp);
                    }
                }
            }
        }

        Level = updatedLevel;
    }
}
