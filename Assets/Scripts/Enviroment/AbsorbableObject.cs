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
    [SerializeField] private List<Renderer> extraRenderers = new List<Renderer>();
    private readonly List<FloatParameter> _parameters = new List<FloatParameter>()
    {
        new FloatParameter()
        {
            name = "Emission",
            propertyName = "EmissionValue",
            startValue = 1,
            endValue = 20
        },
        new FloatParameter()
        {
            name = "Black",
            propertyName = "Black",
            startValue = 0,
            endValue = 0.5f
        }
    };

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
        Renderer rend = GetComponent<MeshRenderer>();
        if (rend != null)
        {
            extraRenderers.Add(rend);
        }
        
        currentEnergy = totalEnergy;
        
        foreach (var parameter in _parameters)
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
        
        foreach (var parameter in _parameters)
        {
            parameter.current = KamUtilities.Map(totalEnergy - currentEnergy,0,totalEnergy, 
                parameter.startValue,parameter.endValue);

            foreach (var r in extraRenderers)
            {
                r.material.SetFloat("_"+parameter.propertyName, parameter.current);
            }
        }

        return result;
    }
}
