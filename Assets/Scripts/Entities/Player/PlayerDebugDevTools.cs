using UnityEngine;

public class PlayerDebugDevTools : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Keybinds")]
    [SerializeField] private KeyCode invincibilityKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode addExpAttack = KeyCode.Alpha2;
    [SerializeField] private KeyCode addExpDefense = KeyCode.Alpha3;
    [SerializeField] private KeyCode addExpMobility = KeyCode.Alpha4;
    [SerializeField] private KeyCode emptyEnergy = KeyCode.Keypad4;
    [SerializeField] private KeyCode fillEnergy = KeyCode.Keypad5;
    [SerializeField] private KeyCode changeAmbientColor = KeyCode.Tab;

    private bool _invincible;
    public bool Invincible { get => _invincible; set => _invincible = value; }

    private void Update()
    {
        DebugInput();
    }

    private void DebugInput()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(invincibilityKey))
            {
                Debug.Log("Debug Tools: Invincible Toggle");
                Invincible = Invincible.Toggle();
            }

            if (Input.GetKeyDown(addExpAttack))
            {
                Debug.Log("Debug Tools: Add Exp Attack");
                player.AddExp(PlayerLevel.ExpType.Attack, 5);
            }

            if (Input.GetKeyDown(addExpDefense))
            {
                Debug.Log("Debug Tools: Add Exp Defense");
                player.AddExp(PlayerLevel.ExpType.Defense, 5);
            }

            if (Input.GetKeyDown(addExpMobility))
            {
                Debug.Log("Debug Tools: Add Exp Mobility");
                player.AddExp(PlayerLevel.ExpType.Mobility, 5);
            }
            
            if (Input.GetKeyDown(emptyEnergy))
            {
                Debug.Log("Debug Tools: Empty Energy");
                player.energy.Current = 0;
            }
            
            if (Input.GetKeyDown(fillEnergy))
            {
                Debug.Log("Debug Tools: Fill Energy");
                player.energy.Current = player.energy.MaxEnergy;
            }
            
            if (Input.GetKeyDown(fillEnergy))
            {
                Debug.Log("Debug Tools: Fill Energy");
                player.energy.Current = player.energy.MaxEnergy;
            }
            
            if (player.input.StartedShootingLeft)
            {
                Debug.Log("Debug Tools: Iterate through left gun");
                player.shoot.SetGun(Shoot.Gun.Left,player.shoot.gunStorage.IterateThroughLeftGun());
            }
            
            if (player.input.StartedShootingRight)
            {
                Debug.Log("Debug Tools: Iterate through right gun");
                player.shoot.SetGun(Shoot.Gun.Right,player.shoot.gunStorage.IterateThroughRightGun());
            }
        }

        if (Input.GetKeyDown(changeAmbientColor))
        {
            ChangeAmbientColor.i.IterateThroughColors();
            
        }
        
    }
}