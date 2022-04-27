using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kam.Utils;

public class AbsorbableObject : MonoBehaviour
{
    [Header("Energy Parameters")]
    [SerializeField] private int totalEnergy;
    private int currentEnergy;
    private bool empty;

    public bool Empty => empty;

    [Header("Effect Parameters")]
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private List<FloatParameter> parameters;

    [Serializable]
    public class FloatParameter
    {
        public string name;
        public string propertyName;
        public float startValue;
        public float endValue;
        private float _current;
        private bool _positive;


        public float current { get => _current; set => _current = value; }
        public bool positive { get => _positive; set => _positive = value; }
    }

    private void Start()
    {
        currentEnergy = totalEnergy;
        
        foreach (var parameter in parameters)
        {
            parameter.current = parameter.startValue;
            parameter.positive = parameter.endValue >= parameter.startValue;
        }
    }

    public int AbsorbTick(int energyTaken)
    {
        int result = 0;
        if (energyTaken > currentEnergy)
        {
            result = currentEnergy;
            currentEnergy = 0;
        }
        else
        {
            result = energyTaken;
            currentEnergy -= energyTaken;
        }

        if(currentEnergy <= 0)
        {
            empty = true;
        }
        
        foreach (var parameter in parameters)
        {
            parameter.current = KamUtilities.Map(totalEnergy - currentEnergy,0,totalEnergy, 
                parameter.startValue,parameter.endValue);

            foreach (var r in renderers)
            {
                r.material.SetFloat("_"+parameter.propertyName, parameter.current);
            }
        }

        return result;
    }
}
