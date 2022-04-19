using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerUpgrades : MonoBehaviour
{
    public class UpgradePool
    {
        public List<Action> Attack = new List<Action>();
        public List<Action> Mobility = new List<Action>();
        public List<Action> Resistance = new List<Action>();

        public List<Action> ChooseUpgradeList(PlayerLevel.ExpType type)
        {
            switch (type)
            {
                case PlayerLevel.ExpType.Attack:
                    return Attack;
                case PlayerLevel.ExpType.Defense:
                    return Resistance;
                case PlayerLevel.ExpType.Mobility:
                    return Mobility;
                
                default:
                    return Attack;
            }
        }
    }

    public enum UpgradeType
    {
        Small, Big
    }
    
    public UpgradePool Small;
    public UpgradePool Big;

    private void Start()
    {
        GeneratePools();
    }

    private void GeneratePools()
    {
        Small = new UpgradePool()
        {
            Attack = new List<Action>()
            {

            },

            Mobility = new List<Action>()
            {

            },

            Resistance = new List<Action>()
            {

            },
        };
        
        Big = new UpgradePool()
        {
            Attack = new List<Action>()
            {

            },

            Mobility = new List<Action>()
            {

            },

            Resistance = new List<Action>()
            {

            },
        };
    }

    public List<Action> GachaPull(UpgradeType type, List<PlayerLevel.Exp> exp)
    {
        List<Action> upgradeOptions = new List<Action>();
        
        UpgradePool pool = new UpgradePool();
        switch (type)
        {
            case UpgradeType.Small:
                pool = Small;
                break;
            case UpgradeType.Big:
                pool = Big;
                break;
        }

        #region Get Top Type exp wise
        PlayerLevel.ExpType topType;
        List<PlayerLevel.ExpType> ties = new List<PlayerLevel.ExpType>();
        int currentMax = 0;
        foreach (PlayerLevel.Exp expType in exp)
        {
            if (expType.ThisLevel > currentMax)
            {
                currentMax = expType.ThisLevel;
                ties.Clear();
                ties.Add(expType.type);
            }
            else if (expType.ThisLevel == currentMax)
            {
                ties.Add(expType.type);
            }
        }
        topType = ties.ChooseRandom();
        #endregion
        
        #region Get top 2 type exp wise
        PlayerLevel.ExpType top2Type = PlayerLevel.ExpType.Attack;
        List<PlayerLevel.ExpType> ties2 = new List<PlayerLevel.ExpType>();
        int currentMax2 = 0;
        foreach (PlayerLevel.Exp expType in exp)
        {
            if (expType.ThisLevel > currentMax2)
            {
                currentMax2 = expType.ThisLevel;
                ties2.Clear();
                ties2.Add(expType.type);
            }
            else if (expType.ThisLevel == currentMax2)
            {
                ties2.Add(expType.type);
            }
        }

        top2Type = ties2.ChooseRandom();
        #endregion

        List<Action> upgradesOfTopType = pool.ChooseUpgradeList(topType);
        List<Action> upgradesOfTop2Type = pool.ChooseUpgradeList(top2Type);
        
        upgradeOptions.Add(upgradesOfTopType.ChooseRandom());
        upgradeOptions.Add(upgradesOfTopType.ChooseRandom());
        upgradeOptions.Add(upgradesOfTop2Type.ChooseRandom());

        return upgradeOptions;
    }
}
