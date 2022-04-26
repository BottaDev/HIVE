using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyDeathEffect : MonoBehaviour
{
    public List<Renderer> renderer;

    [Serializable]
    public class FloatParameter
    {
        public string name;
        public string propertyName;
        public float startValue;
        public float endValue;
        [FormerlySerializedAs("incrementBy")] public float speed;
        private float _current;
        private bool _positive;
        private bool _done;
        
        
        public float current { get => _current; set => _current = value; }
        public bool positive { get => _positive; set => _positive = value; }
        public bool done { get => _done; set => _done = value; }
    }

    public List<FloatParameter> parameters;

    private void Start()
    {
        foreach (var parameter in parameters)
        {
            parameter.current = parameter.startValue;
            parameter.positive = parameter.endValue >= parameter.startValue;
        }
    }

    private void FixedUpdate()
    {
        foreach (var parameter in parameters)
        {
            if (!parameter.done)
            {
                parameter.current = Mathf.Lerp(parameter.current,parameter.endValue,parameter.speed * Time.deltaTime);

                foreach (var r in renderer)
                {
                    r.material.SetFloat("_"+parameter.propertyName, parameter.current);
                }

                bool condition = parameter.positive
                    ? parameter.endValue - parameter.current <= 0
                    : parameter.current - parameter.endValue <= 0;
                
                if (condition)
                {
                    parameter.done = true;
                }
            }
        }
        
    }
}
