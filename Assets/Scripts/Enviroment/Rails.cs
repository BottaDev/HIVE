using System.Collections.Generic;
using UnityEngine;

public class Rails : MonoBehaviour
{
    [Header("Assignables")]
    private Player _p;
    public Player p => _p;

    [Header("Path")] 
    public List<Transform> waypoints;
    private int _current;

    public Transform hook;
    public Transform attachPoint;

    [Header("Settings")]
    public float speed;
    public float detectionRange;

    private bool _waitingForInput;
    private Transform _unattachedParent;
    [HideInInspector] public bool active;
    private bool _reverse;
    
    [Header("Debug")]
    
    public bool loop;

    public void Attach()
    {
        EventManager.Instance.Trigger(EventManager.Events.OnPlayerRailAttached);
        active = true;
        _unattachedParent = p.transform.parent;
        p.transform.parent = attachPoint;
        p.transform.localPosition = Vector3.zero;
    }

    public void UnAttach()
    {
        EventManager.Instance.Trigger(EventManager.Events.OnPlayerRailDeAttached);
        active = false;
        p.transform.parent = _unattachedParent;
        p.movement.rb.velocity = Vector3.zero;
    }
    private void Awake()
    {
        EventManager.Instance.Subscribe(EventManager.Events.SendPlayerReference, GetPlayerReference);
        EventManager.Instance.Trigger(EventManager.Events.NeedsPlayerReference);
    }

    private void GetPlayerReference(params object[] p)
    {
        _p = (Player)p[0];
    }

    private void Update()
    {
        if (_waitingForInput)
        {
            if (p.input.Attaching)
            {
                if(!active)
                {
                    Attach();
                }
                else
                {
                    UnAttach();
                }
            }
            
            if (p.input.Dashing || p.input.Jumping || p.input.Grapple || p.grapple.Pulling || p.input.DirectGrapple)
            {
                UnAttach();
            }
        }

        if (active)
        {
            p.transform.localPosition = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (active)
        {
            MoveTowards(waypoints[_current].position);
        }
    }

    private void MoveTowards(Vector3 pos)
    {
        Vector3 hookPos = new Vector3(hook.position.x, pos.y, hook.position.z);
        float distance = Vector3.Distance(hookPos, pos);
        if (distance < detectionRange)
        {
            hook.position = new Vector3(pos.x, hook.position.y, pos.z);

            if (!_reverse)
            {
                if(_current != waypoints.Count - 1)
                {
                    _current++;
                }
                else if (loop)
                {
                    _reverse = true;
                    _current--;
                }
            }
            else
            {
                if(_current != 0)
                {
                    _current--;
                }
                else if (loop)
                {
                    _reverse = false;
                    _current++;
                }
            }
            
        }
        else
        {
            Vector3 dir = pos - hookPos;
            Vector3 newPos = dir.normalized * speed * Time.deltaTime;
            newPos.y = 0;
            
            hook.position += newPos;
        }
        
    }

    public void WaitForInput(bool state)
    {
        _waitingForInput = state;
    }
}