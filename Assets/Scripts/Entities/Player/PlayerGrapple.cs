using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    public LayerMask grappleable;
    public float maxGrappleDistance = 100f;

    [Header("Joint parameters")]
    [Range(0,100)] public int maxDistance = 80;
    [Range(0,100)] public int minDistance = 25;
    public float springForce = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;
    
    [Header("Assignables")]
    public Player player;
    public LineRenderer lr;
    public Transform firePoint;


    private SpringJoint _joint;
    private Vector3 _currentGrapplePosition;
    private Vector3 _grapplePoint;

    private void Update()
    {
        if (player.input.Grapple)
        {
            StartGrapple();
        }
        else if (player.input.StoppedGrapple)
        {
            StopGrapple();
        }
    }

    //Called after Update
    private void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    ///     Call whenever we want to start a grapple
    /// </summary>
    private void StartGrapple()
    {
        if (Physics.Raycast(player.aim.Ray, out RaycastHit hit, maxGrappleDistance, grappleable))
        {
            _grapplePoint = hit.point;
            _joint = player.gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = _grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.transform.position, _grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            _joint.maxDistance = distanceFromPoint * (maxDistance / 100);
            _joint.minDistance = distanceFromPoint * (minDistance / 100);

            //Adjust these values to fit your game.
            _joint.spring = springForce;
            _joint.damper = damper;
            _joint.massScale = massScale;

            lr.positionCount = 2;
            _currentGrapplePosition = firePoint.position;
        }
    }


    /// <summary>
    ///     Call whenever we want to stop a grapple
    /// </summary>
    private void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(_joint);
    }

    private void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!_joint)
        {
            return;
        }

        _currentGrapplePosition = Vector3.Lerp(_currentGrapplePosition, _grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, firePoint.position);
        lr.SetPosition(1, _currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return _joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return _grapplePoint;
    }
}