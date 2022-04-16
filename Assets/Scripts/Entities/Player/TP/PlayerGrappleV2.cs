using System.Collections;
using UnityEngine;
using System;

public class PlayerGrappleV2 : MonoBehaviour,ITestGrapple
{
    [Header("Parameters")]
    [SerializeField] private LayerMask grappleable;
    [SerializeField] private float hookSpeed = 5f;
    [SerializeField] private float pullSpeed = 0.5f;
    [SerializeField] private float stopDistance = 4f;
    [SerializeField] private float lifetime = 8f;
    
    
    [Header("Assignables")]
    [SerializeField] private Player player;
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private LineRenderer lr;

    
    [Header("Debug")]
    [SerializeField] private bool useStopDistance;
    [SerializeField] private bool destroyAfterLifetime;
    [SerializeField] private bool useGravityWhileAttached;
    [SerializeField] private bool ableToMoveWhileAttached;

    private HookPlayerGrappleV2 _hook;
    private bool _pulling;
    
    public bool Pulling => _pulling;

    // Start is called before the first frame update
    private void Start()
    {
        _pulling = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_hook == null && player.input.Grapple)
        {
            StopAllCoroutines();
            _pulling = false;
            _hook = Instantiate(hookPrefab, shootTransform.position, Quaternion.identity)
                .GetComponent<HookPlayerGrappleV2>();
            _hook.Initialize(transform, this, player.aim.Point, grappleable, hookSpeed, delegate
            {
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
                StartCoroutine(DestroyHookAfterLifetime());
            }
        }
        else if (_hook != null && player.input.StoppedGrapple)
        {
            DestroyHook();
        }

        if (!_pulling || _hook == null)
        {
            return;
        }


        if (Vector3.Distance(transform.position, _hook.transform.position) <= stopDistance && useStopDistance)
        {
            DestroyHook();
        }
        else
        {
            rigid.AddForce((_hook.transform.position - transform.position).normalized * pullSpeed,
                ForceMode.VelocityChange);
        }
    }

    public void StartPull(Action onProximity = null)
    {
        _pulling = true;
    }

    private void DestroyHook()
    {
        if (_hook == null)
        {
            return;
        }
        
        if (!ableToMoveWhileAttached)
        {
            player.movement.ableToMove = true;
        }

        if (!useGravityWhileAttached)
        {
            player.movement.ApplyGravity(true);
        }
        
        _pulling = false;
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

