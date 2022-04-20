using System.Collections;
using UnityEngine;
using System;

public class PlayerGrappleV3 : MonoBehaviour,ITestGrapple
{
    [Header("Parameters")]
    [SerializeField] private LayerMask grappleable;
    [SerializeField] private float hookCD = 0f;
    [SerializeField] private float maxHookDistance = 50f;
    [SerializeField] private float hookSpeed = 5f;
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float pullSpeed = 0.5f;
    [SerializeField] private float minDistancePullMultiplier = 0.5f;
    [SerializeField] private float maxDistancePullMultiplier = 10f;
    [SerializeField] private float lifetime = 8f;
    [SerializeField] private float minDistance = 1f;
    
    [Header("Assignables")]
    [SerializeField] private Player player;
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private LineRenderer lr;

    [Header("Break distance")]
    [SerializeField] private bool useStopDistance;
    [SerializeField] private float stopDistance = 4f;
    
    [Header("Settings")]
    
    [SerializeField] private bool destroyAfterLifetime;
    [SerializeField] private bool useGravityWhileAttached;
    [SerializeField] private bool ableToMoveWhileAttached;
    [SerializeField] private bool resetVelocityOnAttached;
    [SerializeField] private bool resetVelocityOnFinished;
    [SerializeField] private bool sameButtonPressCancel;
    [SerializeField] private bool useExtraForwardForce;
    [SerializeField] private bool useJointDistanceAsForce;
    [SerializeField] private bool useMinDistanceToHook;
    [SerializeField] private bool useDistanceMultiplier;
    [SerializeField] private bool useRigidbodyPosition;

    private HookPlayerGrappleV2 _hook;
    private bool _pulling;
    private Action _onProximity;
    private DistanceJoint3D _joint;
    private float _hookCdCounter;

    public bool Pulling => _pulling;

    // Start is called before the first frame update
    private void Start()
    {
        _pulling = false;
    }

    // Update is called once per frame
    private void Update()
    {
        _hookCdCounter -= Time.deltaTime;
        if (_hook == null && player.input.Grapple && _hookCdCounter < 0f)
        {
            StartHook();
        }
        else if (_hook != null && (sameButtonPressCancel ? player.input.Grapple : player.input.StoppedGrapple))
        {
            DestroyHook();
        }
        
        if (_hook == null)
        {
            return;
        }
        
        float distance = Vector3.Distance(transform.position, _hook.transform.position);
        if (distance > maxHookDistance)
        {
            DestroyHook();
        }
        
        if (!_pulling) 
        {
            return;
        }

        if (player.input.Dashing || player.input.Jumping)
        {
            DestroyHook();
        }
        else if (distance <= stopDistance && useStopDistance)
        {
            DestroyHook();
            _onProximity?.Invoke();
            if (resetVelocityOnFinished)
            {
                rigid.velocity = Vector3.zero;
            }
        }
        else
        {
            if (useJointDistanceAsForce)
            {
                _joint.distance -= pullSpeed * Time.deltaTime;
            }
            else
            {
                Vector3 hookDir = (_hook.transform.position - transform.position).normalized;
                if (useExtraForwardForce)
                {
                    hookDir += player.movement.playerModel.forward * forwardSpeed;
                }

                Vector3 addSpeed = hookDir * pullSpeed * Time.deltaTime;

                if (useDistanceMultiplier)
                {
                    float distanceVelocity = Mathf.Clamp(distance, minDistancePullMultiplier, maxDistancePullMultiplier);
                    addSpeed *= distanceVelocity;
                }

                if (useRigidbodyPosition)
                {
                    rigid.position += addSpeed;
                }
                else
                {
                    rigid.AddForce(addSpeed, ForceMode.VelocityChange);
                }
            }
        }
    }

    public void StartPull(Action onProximity)
    {
        _onProximity = onProximity;
        _pulling = true;
    }

    private void StartHook()
    {
        _pulling = false;
        _hook = Instantiate(hookPrefab, shootTransform.position, Quaternion.identity)
            .GetComponent<HookPlayerGrappleV2>();
        _hook.Initialize(shootTransform, this, player.aim.Point, grappleable, hookSpeed, delegate
        {
            _joint = gameObject.AddComponent<DistanceJoint3D>();
            _joint.connected = _hook.GetComponent<Rigidbody>();
            _joint.self = rigid;
            _joint.spring = 0;
            _joint.damper = 0;
            _joint.ableToExpand = false;
            _joint.ableToShrink = true;

            if (useMinDistanceToHook)
            {
                _joint.useMinDistance = true;
                _joint.minDistance = minDistance;
            }

            if (resetVelocityOnAttached)
            {
                rigid.velocity = Vector3.zero;
            }
            
            if (!ableToMoveWhileAttached)
            {
                player.movement.ableToMove = false;
            }

            if (!useGravityWhileAttached)
            {
                player.movement.ApplyGravity(false);
            }
        });


        if (destroyAfterLifetime)
        {
            StopAllCoroutines();
            StartCoroutine(DestroyHookAfterLifetime());
        }
    }
    private void DestroyHook()
    {
        if (_hook == null)
        {
            return;
        }

        _hookCdCounter = hookCD;
        if (!ableToMoveWhileAttached)
        {
            player.movement.ableToMove = true;
        }

        if (!useGravityWhileAttached)
        {
            player.movement.ApplyGravity(true);
        }

        rigid.velocity.Set(rigid.velocity.x,0,rigid.velocity.z);
        _pulling = false;
        Destroy(_joint);
        Destroy(_hook.gameObject);
        lr.SetPositions(new Vector3[]{});
        _hook = null;
    }

    private IEnumerator DestroyHookAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);

        DestroyHook();
    }
}
    