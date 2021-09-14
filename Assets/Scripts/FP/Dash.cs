using System;
using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private float _dashForce;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _dashDC;

    private Rigidbody _rb;

    private Player _player;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        _dashDC -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashDC <= 0)
            ApplyDash();
    }

    private void ApplyDash()
    {
        _dashDC = 1.5f;
        _rb.AddForce(_player.moveDirection * _dashForce, ForceMode.VelocityChange);
        StartCoroutine(WaitSeconds());
        _rb.velocity = Vector3.zero;
    }
    
    private IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(_dashDuration);
    }
}
