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

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            ApplyDash();
    }

    private void ApplyDash()
    {
        _rb.AddForce(_player.moveDirection * _dashForce, ForceMode.VelocityChange);
        StartCoroutine(WaitSeconds());
        _rb.velocity = Vector3.zero;
    }
    
    private IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(_dashDuration);
    }
}
