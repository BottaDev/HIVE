using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebugDevTools : MonoBehaviour
{
    [SerializeField] Player player;

    [Header("Keybinds")]
    [SerializeField] private KeyCode invincibilityKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode addExpAttack = KeyCode.Alpha2;
    [SerializeField] private KeyCode addExpDefense = KeyCode.Alpha3;
    [SerializeField] private KeyCode addExpMobility = KeyCode.Alpha4;

    bool _invincible;
    public bool invincible { get { return _invincible; } set { _invincible = value; } }

    void Update()
    {
        DebugInput();
    }

    private void DebugInput()
    {
        if (Input.GetKeyDown(invincibilityKey))
        {
            invincible = invincible.Toggle();
        }

        if (Input.GetKeyDown(addExpAttack))
        {
            player.AddEXP(PlayerLevel.EXPType.Attack, 5);
        }

        if (Input.GetKeyDown(addExpDefense))
        {
            player.AddEXP(PlayerLevel.EXPType.Defense, 5);
        }

        if (Input.GetKeyDown(addExpMobility))
        {
            player.AddEXP(PlayerLevel.EXPType.Mobility, 5);
        }
    }
}
