using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevellingSystem
{
    private int _exp;

    private int _level;

    public LevellingSystem(Func<int, int> levelFormula)
    {
        _levelFormula = levelFormula;
        BuildBaseLevelSystem();
        Exp = 0;
    }

    public LevellingSystem(Func<int, int> levelFormula, int exp)
    {
        _levelFormula = levelFormula;
        BuildBaseLevelSystem();
        this.Exp = exp;
    }

    public int Level
    {
        get
        {
            UpdateLevelSystem();
            return _level;
        }
        private set => _level = value;
    }

    public int Exp
    {
        get => _exp;
        set
        {
            _exp = value;
            UpdateLevelSystem();
        }
    }

    public void SetExp(int value, bool updateLevelSystem = true)
    {
        _exp = value;

        if (updateLevelSystem)
        {
            UpdateLevelSystem();
        }
        else
        {
            _level = GetCurrentLevel(_exp);
        }
    }
    public LevellingSystem SetMaxLevel(int newMaxLevel)
    {
        _maxLevel = newMaxLevel;
        BuildBaseLevelSystem();
        return this;
    }

    public LevellingSystem SetStartingLevel(int level)
    {
        _level = level;
        Exp = GetLevelStructure(level).ExpRequirement;
        return this;
    }

    public LevellingSystem SetOnLevelup(Action<int> onLevelup)
    {
        this._onLevelup = onLevelup;
        return this;
    }

    public LevellingSystem AddEveryXLevels(int everyXLevels, Action<int> action)
    {
        _levelTriggers.Add(new EveryXLevels {LevelTrigger = everyXLevels, Action = action});
        return this;
    }

    public void UpdateLevelSystem()
    {
        int updatedLevel = GetCurrentLevel(_exp);

        if (updatedLevel == -1)
        {
            Debug.Log("Could not find level of exp amount " + _exp);
            return;
        }

        if (_level < updatedLevel)
        {
            int difference = updatedLevel - _level;
            for (int i = 0;
                 i < difference;
                 i++)
            {
                int levelUp = _level + i + 1;
                _onLevelup(levelUp);

                for (int j = 0;
                     j < _levelTriggers.Count;
                     j++)
                {
                    if (levelUp % _levelTriggers[j].LevelTrigger == 0)
                    {
                        _levelTriggers[j].Action(levelUp);
                    }
                }
            }
        }

        Level = updatedLevel;
    }

    #region Basic methods and Variables

    public struct LevelStructure
    {
        public int Level;
        public int ExpRequirement;
    }

    public struct EveryXLevels
    {
        public int LevelTrigger;
        public Action<int> Action;
    }

    private List<LevelStructure> _levels;
    private int _maxLevel = 200;
    private Func<int, int> _levelFormula;
    private Action<int> _onLevelup = delegate { };
    private List<EveryXLevels> _levelTriggers = new List<EveryXLevels>();

    public void BuildBaseLevelSystem()
    {
        _levels = new List<LevelStructure>();
        int lastExpRequirement = 0;

        for (int level = 1;
             level <= _maxLevel;
             level++)
        {
            LevelStructure currentLevel = new LevelStructure
            {
                Level = level,
                ExpRequirement = lastExpRequirement + _levelFormula(level)
            };
            lastExpRequirement = currentLevel.ExpRequirement;
            _levels.Add(currentLevel);
        }
    }

    public int GetCurrentLevel(int exp)
    {
        for (int i = 0;
             i < _levels.Count;
             i++)
        {
            LevelStructure currentLevel = _levels[i];

            if (exp < currentLevel.ExpRequirement)
            {
                return _levels[i - 1].Level;
            }
        }

        return -1;
    }

    public int GetDifferenceBetweenLevels(int level1, int level2)
    {
        int lowerLevel = Mathf.Min(level1, level2);
        int higherLevel = Mathf.Max(level1, level2);

        int difference = GetLevelStructure(higherLevel).ExpRequirement - GetLevelStructure(lowerLevel).ExpRequirement;

        return difference;
    }

    public int GetDifferenceBetweenExpAndLevel(int exp, int level)
    {
        return GetLevelStructure(level).ExpRequirement - exp;
    }

    public LevelStructure GetLevelStructure(int level)
    {
        for (int i = 0;
             i < _levels.Count;
             i++)
        {
            if (_levels[i].Level == level)
            {
                return _levels[i];
            }
        }

        Debug.Log("Couldn't find the level of value" + level);
        return new LevelStructure();
    }

    #endregion
}