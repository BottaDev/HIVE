using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Player player;
    
    [Header("Parameters")]
    [SerializeField] private float maxEnergy = 100;
    [SerializeField] private int absorbXPerTick = 1;
    [SerializeField] private float absorbEveryXSeconds = 0.25f;

    private List<AbsorbableObject> absorbableObj = new List<AbsorbableObject>();
    private bool ableToAbsorb;


    private float _current;
    public float Current 
    { 
        get => _current;
        set
        {
            _current = value; 
            EventManager.Instance.Trigger(EventManager.Events.OnEnergyUpdated, _current, MaxEnergy);
        } 
    }
    public float MaxEnergy
    {
        get => maxEnergy;
        set
        {
            maxEnergy = value;  
            EventManager.Instance.Trigger(EventManager.Events.OnEnergyUpdated, _current, MaxEnergy);
        }
    }
    private void Start()
    {
        Current = MaxEnergy;
    }

    private void Update()
    {
        if (Current == MaxEnergy) return;
        
        if (player.input.Absorbing)
        {
            if (ableToAbsorb && !absorbing)
            {
                absorbingCoroutine = StartCoroutine(Absorb());
            }
        }
        else if (absorbing)
        {
            StopCoroutine(absorbingCoroutine);
            absorbingCoroutine = null;
        }
    }

    public void SetMaxEnergy(int maxEnergy)
    {
        float difference =  maxEnergy - this.MaxEnergy;
        AddToMaxEnergy(difference);
    }
    public void AddToMaxEnergy(float amount)
    {
        MaxEnergy += amount;
        Current += amount;
    }
    public bool TakeEnergy(float amount)
    {
        if(!CheckCost(amount)) return false;

        float result = Current - amount;
        this.Current = Mathf.Clamp(result, 0, MaxEnergy);

        return true;
    }
    public void AddEnergy(float amount)
    {
        float result = this.Current + amount;
        this.Current = Mathf.Clamp(result, 0, MaxEnergy);
    }

    bool CheckCost(float amount)
    {
        bool res = amount <= Current;
        Debug.Log("Check cost = " + res);
        return res;
    }

    private Coroutine absorbingCoroutine;
    private bool absorbing { get => absorbingCoroutine != null; } 
    IEnumerator Absorb()
    {
        while (true)
        {
            foreach (var obj in absorbableObj)
            {
                Current += obj.AbsorbTick(absorbXPerTick);
                Current = Mathf.Clamp(Current, 0, MaxEnergy);
            }

            yield return new WaitForSeconds(absorbEveryXSeconds);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AbsorbableObject obj = other.GetComponent<AbsorbableObject>();

        if (obj != null)
        {
            absorbableObj.Add(obj);
            ableToAbsorb = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AbsorbableObject obj = other.GetComponent<AbsorbableObject>();

        if (obj != null)
        {
            absorbableObj.Remove(obj);

            if (absorbableObj.Count == 0)
            {
                ableToAbsorb = false;
            }
        }
    }
}
