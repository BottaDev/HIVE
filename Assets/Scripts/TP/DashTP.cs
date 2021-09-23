using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTP : MonoBehaviour
{
    [SerializeField] [Range(2, 5)] private float _multiplyVelocity;
    [SerializeField] [Range(0.1f, 1)] private float _dashDuration;
    private float _dashCD;

    private PlayerTP _player;

    private void Awake()
    {
        _player = GetComponent<PlayerTP>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCD <= 0)
        {
            StartCoroutine(Cast());
        }

        _dashCD -= Time.deltaTime;
    }

    IEnumerator Cast()
    {
        _player.moveSpeed *= _multiplyVelocity;

        _dashCD = 1.5f;

        yield return new WaitForSeconds(_dashDuration);
        
        _player.moveSpeed /= _multiplyVelocity;
    }
}
