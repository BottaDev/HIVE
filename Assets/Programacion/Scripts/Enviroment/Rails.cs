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
    public float railDashSpeed;
    public float detectionRange;

    private bool _waitingForInput;
    private Transform _unattachedParent;
    [HideInInspector] public bool active;
    private bool _reverse;
    
    [Header("Debug")]
    
    public bool loop;
    public bool returnOnInactive;

    public void Attach()
    {
        EventManager.Instance.Trigger("OnPlayerRailAttached");
        active = true;
        _unattachedParent = p.transform.parent;
        p.attachedRail = this;
        p.jump.CustomAbleToJump = true;
        Vector3 rot = p.transform.rotation.eulerAngles;
        p.transform.parent = attachPoint;
        p.transform.rotation = Quaternion.Euler(rot);
        p.transform.localPosition = Vector3.zero;
    }

    public void UnAttach()
    {
        EventManager.Instance.Trigger("OnPlayerRailDeAttached");
        active = false;
        p.attachedRail = null;
        p.jump.CustomAbleToJump = false;
        p.transform.parent = _unattachedParent;
        p.movement.rb.velocity = Vector3.zero;
    }
    private void Awake()
    {
        EventManager.Instance.Subscribe("SendPlayerReference", GetPlayerReference);
        EventManager.Instance.Trigger("NeedsPlayerReference");
        EventManager.Instance.Unsubscribe("SendPlayerReference", GetPlayerReference);
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

            if (p.input.Jumping || p.hookshot.Pulling || p.input.DirectGrapple)
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
        else if (_current >= 0 && returnOnInactive)
        {
            _reverse = true;
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
                else if (loop && active)
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