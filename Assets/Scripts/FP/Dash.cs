using System;
using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private float _dashForce;
    [SerializeField] private float _dashDuration;

    private Rigidbody _rb;

    private Player _player;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Cast());
        }
    }

    IEnumerator Cast()
    {
        _rb.AddForce(_player.moveDirection * _dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(_dashDuration);
        
        _rb.velocity = Vector3.zero;
    }
}
