using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Rails : MonoBehaviour
{
    [Header("Assignables")]
    private Player _p;
    public Player p => _p;

    [Header("Path")] 
    public List<Transform> waypoints;
    private int _current;
    private bool finishedWithPath;

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

    [Header("Animations")]
    public Animator anim;
    public string onGrab;
    public string onRelease;
    public string onEndMovement;
    
    public List<string> onIdleVariation;
    public float minTimeForVariation;
    public float maxTimeForVariation;
    
    

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
        finishedWithPath = false;
        
        
        anim.SetTrigger(onGrab);
    }

    public void UnAttach()
    {
        EventManager.Instance.Trigger("OnPlayerRailDeAttached");
        active = false;
        p.attachedRail = null;
        p.jump.CustomAbleToJump = false;
        p.transform.parent = _unattachedParent;
        p.movement.rb.velocity = Vector3.zero;
        
        anim.SetTrigger(onRelease);
    }
    
    private void Awake()
    {
        EventManager.Instance.Subscribe("SendPlayerReference", GetPlayerReference);
        EventManager.Instance.Trigger("NeedsPlayerReference");
        EventManager.Instance.Unsubscribe("SendPlayerReference", GetPlayerReference);

        StartCoroutine(IdleAnimationVariation(minTimeForVariation, maxTimeForVariation, onIdleVariation));
        finishedWithPath = true;
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

            if (active)
            {
                if (p.input.Jumping || p.hookshot.Pulling || p.input.DirectGrapple)
                {
                    UnAttach();
                }
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
            if (_current < waypoints.Count - 1)
            {
                MoveTowards(waypoints[_current].position);
            }
            else if(_current == waypoints.Count - 1)
            {
                MoveTowards(waypoints[_current].position, delegate
                {
                    UnAttach();
                    p.movement.rb.AddForce(transform.forward * 10, ForceMode.Impulse);
                    p.movement.rb.AddForce(transform.up * 10, ForceMode.Impulse);
                });
            }
        }
        else if(!finishedWithPath)
        {
            if (_current > 0)
            {
                _reverse = true;
                MoveTowards(waypoints[_current].position);
            }
            else if(_current == 0)
            {
                MoveTowards(waypoints[_current].position, delegate
                {
                    _reverse = false;
                    anim.SetTrigger(onEndMovement);
                    finishedWithPath = true;
                });
            }
        }
    }

    private void MoveTowards(Vector3 pos, Action onArrive = null)
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
            }
            else
            {
                if(_current != 0)
                {
                    _current--;
                }
            }
            
            onArrive?.Invoke();
        }
        else
        {
            Vector3 dir = pos - hookPos;
            Vector3 newPos = dir.normalized * speed * Time.deltaTime;
            newPos.y = 0;

            Quaternion rotGoal = Quaternion.LookRotation(dir);
            hook.rotation = Quaternion.Slerp(hook.rotation, rotGoal, 0.05f);
            
            hook.position += newPos;
        }
        
    }

    public void WaitForInput(bool state)
    {
        _waitingForInput = state;
    }

    IEnumerator IdleAnimationVariation(float minTime, float maxTime, List<string> variations)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));

            if (!active && finishedWithPath)
            {
                anim.SetTrigger(variations.ChooseRandom());
            }
        }
    }
}