using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 100;

    private float current;
    public float Current { get => current; }
    public int Max { get => maxEnergy; }
    private void Start()
    {
        current = maxEnergy;
    }

    public void SetMaxEnergy(int maxEnergy)
    {
        int difference =  maxEnergy - this.maxEnergy;
        AddToMaxEnergy(difference);
    }
    public void AddToMaxEnergy(int amount)
    {
        maxEnergy += amount;
        current += amount;
    }
    public bool TakeEnergy(float amount)
    {
        if(!CheckCost(amount)) return false;

        float result = current - amount;
        this.current = Mathf.Clamp(result, 0, maxEnergy);

        return true;
    }
    public void AddEnergy(float amount)
    {
        float result = this.current + amount;
        this.current = Mathf.Clamp(result, 0, maxEnergy);
    }

    bool CheckCost(float amount)
    {
        bool res = amount <= current;
        Debug.Log("Check cost = " + res);
        return res;
    }
}
