using System;
using UnityEngine;

public class HookPlayerGrappleV2 : MonoBehaviour
{
    private float _speed;
    private ITestGrapple _grapple;
    private Transform _grappleTransform;
    [SerializeField] private LineRenderer lineRenderer;
    private Vector3 _prevPos;
    private LayerMask _mask;
    private Action _onAttached;
    private bool _frozen;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Vector3[] positions =
        {
            transform.position,
            _grappleTransform.position
        };

        lineRenderer.SetPositions(positions);
    }

    private void FixedUpdate()
    {
        if (!_frozen)
        {
            MoveToPosition();
        }
    }

    private void MoveToPosition()
    {
        _prevPos = transform.position;
        transform.Translate(Vector3.forward * Time.deltaTime * _speed);

        Vector3 dir = (transform.position - _prevPos);
        
        RaycastHitGameobject(dir);
    }

    private void RaycastHitGameobject(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dir.magnitude, _mask))
        {
            //check if its the layer RailTrigger
            if (hit.transform.gameObject.layer == 13)
            {
                Rails rail = hit.transform.GetComponentInParent<Rails>();
                transform.position = rail.hook.position;
                _grapple.StartPull(delegate { rail.Attach(); });
            }
            else
            {
                transform.position = hit.point;
                _grapple.StartPull();
            }
            
            _onAttached?.Invoke();
            
            _frozen = true;
        }
    }

    public void Initialize(Transform transform, ITestGrapple grapple, Vector3 aimingPoint, LayerMask mask, float speed, Action onAttach = null)
    {
        this.transform.LookAt(aimingPoint);
        this._grapple = grapple;
        _grappleTransform = transform;
        this._mask = mask;
        this._speed = speed;
        _onAttached = onAttach;
        _frozen = false;
    }
    
}
