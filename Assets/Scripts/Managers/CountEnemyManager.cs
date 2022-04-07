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
    public TextMeshProUGUI UIEnemyCounter;

    private void Awake()
    {
        EventManager.Instance.Subscribe(EventManager.Events.OnEnemyCounted, OnEnemyCounted);
        EventManager.Instance.Subscribe(EventManager.Events.OnTotalEnemy, OnTotalEnemy);
    }

    private void UpdateText()
    {
        UIEnemyCounter.text = _currentEnemies.ToString() + " / " + _totalEnemies.ToString();
    }

    private void OnEnemyCounted(params object[] parameters)
    {
        var enemyCount = (int) parameters[0];

        _currentEnemies = enemyCount;

        UpdateText();
    }

    private void OnTotalEnemy(params object[] parameters)
    {
        var totalEnemies = (int) parameters[0];

        _totalEnemies = totalEnemies;

        UpdateText();

        EventManager.Instance.Unsubscribe(EventManager.Events.OnTotalEnemy, OnTotalEnemy);
    }
}
