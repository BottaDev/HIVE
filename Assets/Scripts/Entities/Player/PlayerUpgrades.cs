using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerUpgrades : MonoBehaviour
{
    public class UpgradePool
    {
        public List<Upgrade> Attack = new List<Upgrade>();
        public List<Upgrade> Mobility = new List<Upgrade>();
        public List<Upgrade> Resistance = new List<Upgrade>();

        public List<Upgrade> ChooseUpgradeList(PlayerLevel.ExpType type)
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
    public class Upgrade
    {
        public string name;
        public string description;
        public Action action = delegate { };
    }

    public enum UpgradeType
    {
        Small, Big
    }

    public Player player;

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
            Attack = new List<Upgrade>()
            {
                new Upgrade()
                {
                    name = "Attack Testing Upgrade",
                    description = "Just a testing upgrade.",
                    action = Test1
                }
            },

            Mobility = new List<Upgrade>()
            {
                new Upgrade()
                {
                    name = "Mobility Testing Upgrade",
                    description = "Just a testing upgrade.",
                    action = Test3
                }
            },

            Resistance = new List<Upgrade>()
            {
                new Upgrade()
                {
                    name = "Resistance Testing Upgrade",
                    description = "Just a testing upgrade.",
                    action = Test2
                }
            },
        };
        
        Big = new UpgradePool()
        {
            Attack = new List<Upgrade>()
            {
                
            },

            Mobility = new List<Upgrade>()
            {
                new Upgrade()
                {
                    name = "Double Jump",
                    description = "Unlocks Double Jump",
                    action = ActivateDoubleJump
                }
            },

            Resistance = new List<Upgrade>()
            {

            },
        };
    }

    public List<Upgrade> GachaPull(UpgradeType type, List<PlayerLevel.Exp> exp)
    {
        List<Upgrade> upgradeOptions = new List<Upgrade>();
        
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
            if (expType.ThisLevel > currentMax2 && expType.type != topType)
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

        List<Upgrade> upgradesOfTopType = pool.ChooseUpgradeList(topType);
        List<Upgrade> upgradesOfTop2Type = pool.ChooseUpgradeList(top2Type);

        Upgrade empty = new Upgrade();
        if (upgradesOfTopType.Count > 0)
        {
            upgradeOptions.Add(upgradesOfTopType.ChooseRandom());
            upgradeOptions.Add(upgradesOfTopType.ChooseRandom());
        }
        else
        {
            upgradeOptions.Add(empty);
            upgradeOptions.Add(empty);
        }

        if (upgradesOfTop2Type.Count > 0)
        {
            upgradeOptions.Add(upgradesOfTop2Type.ChooseRandom());
        }
        else
        {
            upgradeOptions.Add(empty);
        }

        

        return upgradeOptions;
    }

    #region UpgradesSmall
    void Test1()
    {
        Debug.Log("ATTACK TEST UPGRADE TRIGGER");
    }

    void Test2()
    {
        Debug.Log("RESISTANCE TEST UPGRADE TRIGGER");
    }

    void Test3()
    {
        Debug.Log("MOBILITY TEST UPGRADE TRIGGER");
    }

    #endregion

    #region UpgradesBig
    void ActivateDoubleJump()
    {
        player.jump.amountOfJumps = 2;
    }

    #endregion
}
