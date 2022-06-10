using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class PlayerHookObject : MonoBehaviour
{
    private float _speed;
    private PlayerDirectHookshot _grapple;
    private Transform _grappleTransform;
    [SerializeField] private LineRenderer lineRenderer;
    private Vector3 _prevPos;
    private LayerMask _directMask;
    private LayerMask _indirectMask;
    private Action _onAttached;
    private bool _frozen;

    private bool _firstFrame = true;
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
        if (_firstFrame)
        {
            _firstFrame = false;
            //Check if you're colliding with something on the first frame you're created.
            Vector3 translation = transform.forward * Time.deltaTime * _speed;

            RaycastHitGameobject(translation);
            return;
        }
        
        _prevPos = transform.position;
        transform.Translate(Vector3.forward * Time.deltaTime * _speed);

        Vector3 dir = (transform.position - _prevPos);
        
        RaycastHitGameobject(dir);
    }

    private Vector3 lastDir;
    private void RaycastHitGameobject(Vector3 dir)
    {
        lastDir = dir;
        
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, dir.magnitude, _directMask))
        {
            //check if its the layer RailTrigger
            if (hit.transform.gameObject.layer == 13)
            {
                Rails rail = hit.transform.GetComponentInParent<Rails>();
                transform.position = rail.hook.position;
                _grapple.StartPull(PlayerDirectHookshot.HookType.Direct, delegate { rail.Attach(); });
            }
            else
            {
                transform.position = hit.point;
                _grapple.StartPull(PlayerDirectHookshot.HookType.Direct);
            }
            
            _onAttached?.Invoke();
            
            _frozen = true;
        }
        else
        {
            if (Physics.Raycast(transform.position, dir, out RaycastHit indirectHit, dir.magnitude, _indirectMask))
            {
                transform.position = indirectHit.point;
                _grapple.StartPull(PlayerDirectHookshot.HookType.Indirect);
                _onAttached?.Invoke();
            
                _frozen = true;
            }
        }
    }

    public void Initialize(Transform transform, PlayerDirectHookshot grapple, Vector3 aimingPoint, LayerMask directMask,LayerMask indirectMask, float speed, Action onAttach = null)
    {
        this.transform.LookAt(aimingPoint);
        this._grapple = grapple;
        _grappleTransform = transform;
        this._directMask = directMask;
        _indirectMask = indirectMask;
        this._speed = speed;
        _onAttached = onAttach;
        _frozen = false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + lastDir);
        
    }
}

#region CUSTOM_EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(PlayerHookObject))]
public class KamCustomEditor_PlayerHookObject : KamCustomEditor
{

}
#endif
#endregion