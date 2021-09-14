using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTP : MonoBehaviour
{
    [SerializeField] private float _dashForce;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _dashCD;

    private Rigidbody _rb;

    private PlayerTP _player;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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
        _rb.AddForce(_player.moveDirection * _dashForce, ForceMode.VelocityChange);

        _dashCD = 1.5f;

        yield return new WaitForSeconds(_dashDuration);
        
        _rb.velocity = Vector3.zero;
    }
}
