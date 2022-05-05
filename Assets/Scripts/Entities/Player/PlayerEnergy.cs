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

    [Header("Prompt")]
    [SerializeField] private string message;
    [SerializeField] private Color messageColor;
    private bool promptShowing;

    [Header("Effect")]
    [SerializeField] private string absorbablePivotTag;
    [SerializeField] private Electric electricityPrefab;
    private List<Electric> effects = new List<Electric>();
    private bool effectActive;

    private List<AbsorbableObject> absorbableObj = new List<AbsorbableObject>();
    private bool ableToAbsorb;


    private float _current;
    public float Current 
    { 
        get => _current;
        set
        {
            _current = value; 
            EventManager.Instance.Trigger("OnEnergyUpdated", _current, MaxEnergy);
        } 
    }
    public float MaxEnergy
    {
        get => maxEnergy;
        set
        {
            maxEnergy = value;  
            EventManager.Instance.Trigger("OnEnergyUpdated", _current, MaxEnergy);
        }
    }
    private void Start()
    {
        Current = MaxEnergy;
    }

    private void Update()
    {
        EmptyCheck();

        if(Current == MaxEnergy)
        {
            if (ableToAbsorb)
            {
                ableToAbsorb = false;
            }
        }
        else
        {
            if (!ableToAbsorb && absorbableObj.Count > 0)
            {
                ableToAbsorb = true;
            }
        }
        
        if (absorbing)
        {
            if (!effectActive)
            {
                for (int i = 0; i < absorbableObj.Count; i++)
                {
                    Electric effect = Instantiate(electricityPrefab);
                    effect.transformPointB = player.transform;

                    
                    Transform a = absorbableObj[i].CompareTag(absorbablePivotTag) ? absorbableObj[i].transform : absorbableObj[i].transform.parent;
                    effect.transformPointA = a;

                    effects.Add(effect);
                }

                effectActive = true;
            }


            if (!promptShowing)
            {
                promptShowing = true;
                EventManager.Instance.Trigger("OnSendUIMessage", message, messageColor);
            }
        }
        else
        {
            if (effectActive)
            {
                foreach (var effect in effects)
                {
                    Destroy(effect.gameObject);
                }
                effects.Clear();

                effectActive = false;
            }

            if (promptShowing)
            {
                EventManager.Instance.Trigger("OnEliminateUIMessage", message);
                promptShowing = false;
            }
        }


        if (ableToAbsorb && !absorbing)
        {
            absorbingCoroutine = StartCoroutine(Absorb());
        }
        else if (absorbing && !ableToAbsorb)
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

    private void EmptyCheck()
    {
        List<AbsorbableObject> empty = new List<AbsorbableObject>();
        foreach (var obj in absorbableObj)
        {
            if (obj.Empty)
            {
                empty.Add(obj);

                if (absorbableObj.Count == empty.Count)
                {
                    HidePrompt();
                    ableToAbsorb = false;
                }
            }
        }

        foreach (var obj in empty)
        {
            absorbableObj.Remove(obj);
        }
    }
    private void ShowPrompt()
    {
        if (!promptShowing)
        {
            promptShowing = true;
            EventManager.Instance.Trigger("OnSendUIMessage", message, messageColor);
        }
    }

    private void HidePrompt()
    {
        if (promptShowing)
        {
            EventManager.Instance.Trigger("OnEliminateUIMessage", message);
            promptShowing = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AbsorbableObject obj = other.GetComponent<AbsorbableObject>();

        if (obj != null && !obj.Empty)
        {
            absorbableObj.Add(obj);
            ableToAbsorb = true;

            if(Current != MaxEnergy)
            {
                ShowPrompt();
            }
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

                HidePrompt();
                
            }
        }
    }
}
