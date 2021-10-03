using System.Collections;
using UnityEngine;

public class DashTP : MonoBehaviour
{
    [SerializeField] [Range(2, 5)] private float _multiplyVelocity;
    [SerializeField] [Range(5, 10)] private float _airMultiplyVelocity;
    [SerializeField] [Range(0.1f, 1)] private float _dashDuration;
    [SerializeField] private TrailRenderer[] trails;
    [SerializeField] private float _dashCD;
    private float _currentDashCD;

    private Camera _cam;
    private Player _player;

    private void Awake()
    {
        _cam = Camera.main;
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _currentDashCD <= 0)
        {
            StartCoroutine(Cast());
            StartCoroutine(CameraEffect());
        }

        _currentDashCD -= Time.deltaTime;
    }

    IEnumerator Cast()
    {
        foreach (TrailRenderer item in trails)
            item.emitting = true;

        if (_player.isGrounded)
            _player.moveSpeed *= _multiplyVelocity;
        else
            _player.moveSpeed *= _airMultiplyVelocity;

        _currentDashCD = _dashCD;

        yield return new WaitForSeconds(_dashDuration);

        foreach (TrailRenderer item in trails)
            item.emitting = false;

        if (_player.isGrounded)
            _player.moveSpeed /= _multiplyVelocity;
        else
            _player.moveSpeed /= _airMultiplyVelocity;
    }

    IEnumerator CameraEffect()
    {
        float steps = 10;

        float speedTime = _dashDuration / 2f;
        float slowTime  = _dashDuration - speedTime;

        for (int i = 0; i < steps; i++)
        {
            _cam.fieldOfView += 1;
            yield return new WaitForSeconds(speedTime/steps);
        }

        for (int i = 0; i < steps; i++)
        {
            _cam.fieldOfView -= 1;
            yield return new WaitForSeconds(slowTime/steps);
        }
    }
}
