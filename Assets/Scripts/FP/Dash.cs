using System;
using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] [Range(2, 5)] private float _multiplyVelocity;
    [SerializeField] [Range(0.1f, 1)] private float _dashDuration;
    private float _dashCD;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        _dashCD -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCD <= 0)
            StartCoroutine(WaitSeconds());
    }
    
    private IEnumerator WaitSeconds()
    {
        _player.baseSpeed *= _multiplyVelocity;

        _dashCD = 1.5f;

        yield return new WaitForSeconds(_dashDuration);
        
        _player.baseSpeed /= _multiplyVelocity;
    }
}
