using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGunStorage : MonoBehaviour
{
    [SerializeField] private List<Gun> storage;

    [System.Serializable]
    public struct Gun
    {
        public string name;
        public Guns enumName;
        public PlayerGun gun;
    }
    public enum Guns
    {
        Default,
        Shotgun,
        Burst,
        MachineGun,
        None
    }

    public void TurnOffAllGuns()
    {
        foreach (var gun in storage)
        {
            gun.gun.gameObject.SetActive(false);
        }
    }
    public PlayerGun GetGun(Guns gun)
    {
        if (gun == Guns.None)
        {
            return null;
        }
        
        return storage.First(x => x.enumName == gun).gun;
    }

    private int currentLeft = 0;
    public PlayerGun IterateThroughLeftGun()
    {
        currentLeft++;
        
        if (currentLeft >= storage.Count)
        {
            currentLeft = 0;
        }
        
        return storage[currentLeft].gun;
    }
    
    private int currentRight = 0;
    public PlayerGun IterateThroughRightGun()
    {
        currentRight++;
        
        if (currentRight >= storage.Count)
        {
            currentRight = 0;
        }
        
        return storage[currentRight].gun;
    }
}
