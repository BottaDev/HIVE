using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class IAManager : MonoBehaviour
{
    private static IAManager _instance;

    public static IAManager Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    public Transform target;
    public float radiusAroundTarget = 0.5f;
    public List<AI> Units = new List<AI>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }
    public void MakeAgetsCircleTarget()
    {
        for (int i = 0; i < Units.Count; i++)
        {
            Units[i].MoveTo(new Vector3(
                target.position.x + radiusAroundTarget * Mathf.Cos(2 * Mathf.PI * i / Units.Count),
                target.position.y,
                target.position.z + radiusAroundTarget * Mathf.Sin(2 * Mathf.PI * i / Units.Count))
                );
        }
    }

    private void Update()
    {
        MakeAgetsCircleTarget();
    }

}
