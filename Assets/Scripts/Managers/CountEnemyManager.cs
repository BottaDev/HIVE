using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CountEnemyManager : MonoBehaviour
{
    private int _currentEnemies;
    private int _totalEnemies;
    public TextMeshProUGUI currentEnemies;
    public TextMeshProUGUI totalEnemies;
    
    private void Awake()
    {
        EventManager.Instance.Subscribe("OnEnemyCounted", OnEnemyCounted);
        EventManager.Instance.Subscribe("OnTotalEnemy", OnTotalEnemy);
    }

    private void Update()
    {
        currentEnemies.text = _currentEnemies.ToString();
        totalEnemies.text = _totalEnemies.ToString();
    }

    private void OnEnemyCounted(params object[] parameters)
    {
        var enemyCount = (int) parameters[0];

        _currentEnemies = enemyCount;
    }

    private void OnTotalEnemy(params object[] parameters)
    {
        var totalEnemies = (int) parameters[0];

        _totalEnemies = totalEnemies;
        
        EventManager.Instance.Unsubscribe("OnTotalEnemy", OnTotalEnemy);
    }
}
