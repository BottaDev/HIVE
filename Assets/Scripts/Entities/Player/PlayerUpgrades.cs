using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

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

        public void SetTypeOfUpgrades()
        {
            foreach (var upgrade in Attack)
            {
                upgrade.type = PlayerLevel.ExpType.Attack;
            }
            
            foreach (var upgrade in Mobility)
            {
                upgrade.type = PlayerLevel.ExpType.Mobility;
            }
            
            foreach (var upgrade in Resistance)
            {
                upgrade.type = PlayerLevel.ExpType.Defense;
            }
        }
    }
    public class Upgrade
    {
        public string name;
        public string description;
        public string longDescription;
        public Action<Player> action = delegate { };
        public bool oneTimeOnly = true;
        public PlayerLevel.ExpType type;
        public Sprite icon;
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
            Attack = new List<Upgrade>()
            {
                new Upgrade()
                {
                    name = "DMG Buff",
                    description = "+20% DMG",
                    longDescription = "+20% Damage based on current damage stat.",
                    action = DamagePercentBuff,
                    oneTimeOnly = false, 
                    icon = AssetDatabase.i.GetUpgradeIcon(Icons.Attack)
                }
            },

            Mobility = new List<Upgrade>()
            {
                new Upgrade()
                {
                    name = "Speed Buff",
                    description = "+5% Mobility",
                    longDescription = "+5% Max Speed based on current max speed stat.",
                    action = MobilityPercentBuff,
                    oneTimeOnly = false, 
                    icon = AssetDatabase.i.GetUpgradeIcon(Icons.Mobility)
                }
            },

            Resistance = new List<Upgrade>()
            {
                new Upgrade()
                {
                    name = "HP Buff",
                    description = "+5 HP",
                    longDescription = "+5 Max Hit points (Current hit points are also affected)",
                    action = HPFlatUpgrade,
                    oneTimeOnly = false, 
                    icon = AssetDatabase.i.GetUpgradeIcon(Icons.Defense)
                }
            },
        };
        Small.SetTypeOfUpgrades();

        Big = new UpgradePool()
        {
            Attack = new List<Upgrade>()
            {
                new Upgrade()
                {
                    name = "Grenade Throw",
                    description = "Unlocks Grenade Throw",
                    longDescription = "Unlocks a grenade throw mechanic, the grenade has a great range and does 20 damage on hit. It has a 15 seconds cooldown and will explode on contact with any enemy or after 3 seconds of being thrown.",
                    action = ActivateGrenadeThrow, 
                    icon = AssetDatabase.i.GetUpgradeIcon(Icons.Attack)
                }
            },

            Mobility = new List<Upgrade>()
            {
                new Upgrade()
                {
                    name = "Double Jump",
                    description = "Unlocks Double Jump",
                    longDescription = "You read this like there was any reason to expand on it. It is a double jump. Go double jump.",
                    action = ActivateDoubleJump, 
                    icon = AssetDatabase.i.GetUpgradeIcon(Icons.Mobility)
                }
            },

            Resistance = new List<Upgrade>()
            {

            },
        };
        Big.SetTypeOfUpgrades();
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
        
        PlayerLevel.ExpType topType = exp.GetMax((x => x.ThisLevel), true).type;
        
        PlayerLevel.ExpType top2Type = exp.Where(x => x.type != topType).ToList()
            .GetMax((x => x.ThisLevel), true).type;

        List<Upgrade> upgradesOfTopType = pool.ChooseUpgradeList(topType);
        List<Upgrade> upgradesOfTop2Type = pool.ChooseUpgradeList(top2Type);

        Upgrade empty = new Upgrade();
        empty.type = topType;
        if (upgradesOfTopType.Count > 0)
        {
            Upgrade one = upgradesOfTopType.ChooseRandom();
            upgradeOptions.Add(one);

            if (one.oneTimeOnly)
            {
                upgradesOfTopType.Remove(one);
            }
            

            
            List<Upgrade> newList = upgradesOfTopType.Where(x => x != one).ToList();
            if (newList.Count > 0)
            {
                Upgrade two = newList.ChooseRandom();
                upgradeOptions.Add(two);
                if (two.oneTimeOnly)
                {
                    upgradesOfTopType.Remove(two);
                }
            }
            else
            {
                
                upgradeOptions.Add(empty);
            }
        }
        else
        {
            upgradeOptions.Add(empty);
            upgradeOptions.Add(empty);
        }

        if (upgradesOfTop2Type.Count > 0)
        {
            Upgrade one = upgradesOfTop2Type.ChooseRandom();
            upgradeOptions.Add(one);
            
            if (one.oneTimeOnly)
            {
                upgradesOfTop2Type.Remove(one);
            }
        }
        else
        {
            Upgrade empty2 = new Upgrade();
            empty2.type = top2Type;
            upgradeOptions.Add(empty2);
        }


        //DEBUG TEST
        bool first = true;
        PlayerLevel.ExpType checktype = PlayerLevel.ExpType.Attack;
        int count = 0;
        foreach (var item in upgradeOptions)
        {
            if (first)
            {
                checktype = item.type;
                first = false;
            }

            if(item.type == checktype)
            {
                count++;
            }
            
        }

        if(count > 2)
        {
            Debug.LogError($"Bug: TOP - {topType.ToString()}; TOP 2 - {top2Type.ToString()}");
        }

        return upgradeOptions;
    }

    #region UpgradesSmall
    void DamagePercentBuff(Player player)
    {
        float percentageUpgrade = 20;
        
        player.shoot.gunStorage.ForAllGuns(
            delegate(PlayerGun gun)
            {
                float increase = (gun.damage * percentageUpgrade) / 100;
                gun.damage += increase > 1 ? (int) increase : 1;
            }
        );
    }

    void HPFlatUpgrade(Player player)
    {
        int buff = 5;
        player.MaxHP += buff;
    }

    void MobilityPercentBuff(Player player)
    {
        float percentageUpgrade = 5;
        player.movement.maxSpeed += (player.movement.maxSpeed * percentageUpgrade)/100;
    }

    #endregion

    #region UpgradesBig
    void ActivateDoubleJump(Player player)
    {
        player.jump.amountOfJumps = 2;
    }

    void ActivateGrenadeThrow(Player player)
    {
        player.grenadeThrow.Unlock();
    }
    #endregion
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(PlayerUpgrades))]
public class KamCustomEditor_PlayerUpgrades : KamCustomEditor
{
    
}
#endif
#endregion