using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rails : MonoBehaviour
{
    [Header("Path")] 
    public List<Transform> waypoints;
    private int current = 0;

    public Transform hook;
    public Transform attachPoint;

    [Header("Settings")]
    public float speed;
    public float detectionRange;

    private Transform player;
    private Transform unattachedParent;
    private bool active;
    [Header("Debug")]
    public Player p;
    public bool loop;

    public void Attach(Transform player)
    {
        if (!active)
        {
            active = true;
            unattachedParent = player.parent;
            this.player = player;
            player.parent = attachPoint;
            player.localPosition = Vector3.zero;
        }
    }

    public void UnAttach()
    {
        if (active)
        {
            active = false;
            player.parent = unattachedParent;
            player = null;
        }
    }

    private void Update()
    {
        if (p.input.isMoving || p.input.dashing)
        {
            UnAttach();
        }

        if (active)
        {
            player.localPosition = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (active)
        {
            MoveTowards(waypoints[current].position);
        }
    }

    public void MoveTowards(Vector3 pos)
    {
        float distance = Vector3.Distance(hook.position, pos);
        if (distance < detectionRange)
        {
            hook.position = new Vector3(pos.x, hook.position.y, pos.z);

            if(current != waypoints.Count - 1)
            {
                current++;
            }
            else if (loop)
            {
                current = 0;
            }
        }
        else
        {
            Vector3 dir = pos - hook.position;
            Vector3 newPos = dir.normalized * speed * Time.deltaTime;
            newPos.y = 0;

            hook.position += newPos;
        }
        
    }
}