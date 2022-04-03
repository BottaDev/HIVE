using System.Collections;
using UnityEngine;

public class DashTP : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private PlayerInput        input;
    [SerializeField] private PlayerMovement     movement;
    [SerializeField] private Player             player;
    [SerializeField] private TrailRenderer[]    trails;
    private Camera _cam;

    //Get whatever information you need for this script
    private bool dashing { get { return input.dashing; } }


    [Header("Parameters")]
    [Tooltip("Speed during dash")][SerializeField] 
    private float _dashVelocity;
    [Tooltip("Time you stay in dash Velocity.")][SerializeField] 
    private float _dashDuration;
    [Tooltip("Time it takes for you to be able to dash again after cast.")][SerializeField] 
    private float _dashCD;


    private float _currentDashCD;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    { 
        if (dashing && _currentDashCD <= 0)
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

        movement.ableToMove = false;
        _currentDashCD = _dashCD;
        movement.ApplyGravity(false);

        Vector3 originalVelocity = movement.rb.velocity;
        Vector3 orientation = new Vector3(originalVelocity.x, 0, originalVelocity.z);
        if(orientation.magnitude == 0)
        {
            orientation = movement.playerModel.forward;
        }

        Vector3 dashdirection = orientation.normalized;
        movement.rb.velocity = dashdirection * _dashVelocity;

        yield return new WaitForSeconds(_dashDuration);

        foreach (TrailRenderer item in trails)
            item.emitting = false;

        dashdirection = movement.rb.velocity.normalized;
        
        movement.rb.velocity = dashdirection * originalVelocity.magnitude;
        movement.ableToMove = true;
        movement.ApplyGravity(true);
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
