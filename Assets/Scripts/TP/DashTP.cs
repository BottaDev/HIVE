using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTP : MonoBehaviour
{
    [SerializeField] [Range(2, 5)] private float _multiplyVelocity;
    [SerializeField] [Range(0.1f, 1)] private float _dashDuration;
    [SerializeField] private TrailRenderer[] trails;
    private float _dashCD;
    private Camera _cam;

    private PlayerTP _player;

    private void Awake()
    {
        _cam = Camera.main;
        _player = GetComponent<PlayerTP>();
    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCD <= 0)
        {
            StartCoroutine(Cast());
            StartCoroutine(CameraEffect());
        }

        _dashCD -= Time.deltaTime;


    }

    IEnumerator Cast()
    {
        foreach (TrailRenderer item in trails)
            item.emitting = true;
        

        _player.moveSpeed *= _multiplyVelocity;

        _dashCD = 1.5f;

        yield return new WaitForSeconds(_dashDuration);

        foreach (TrailRenderer item in trails)
            item.emitting = false;

        _player.moveSpeed /= _multiplyVelocity;
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
