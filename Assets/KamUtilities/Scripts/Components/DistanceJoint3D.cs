using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceJoint3D : MonoBehaviour
{
    public Rigidbody self;
    public Rigidbody connected;
    public bool determineDistanceAtStart = true;
    public float distance;
    public float spring = 0.1f;
    public float damper = 5f;
    public bool ableToShrink;
    public bool ableToExpand;
    
    private void Start()
    {
        if (determineDistanceAtStart)
        {
            distance = Vector3.Distance(self.position, connected.position);
        }
    }
    
   private  void FixedUpdate()
   {
       var newDistance = Vector3.Distance(self.position, connected.position);

       bool expandingLimit = newDistance > distance && !ableToExpand;
       bool shrinkingLimit = newDistance < distance && !ableToShrink;
       
       if (expandingLimit || shrinkingLimit)
       {
           Limit();
       }
       else
       {
           distance = newDistance;
       }
   }

   private void Limit()
   {
       var connection = self.position - connected.position;
       var distanceDiscrepancy = distance - connection.magnitude;

       self.position += distanceDiscrepancy * connection.normalized;

       var velocityTarget = connection + (self.velocity + Physics.gravity * spring);
       var projectOnConnection = Vector3.Project(velocityTarget, connection);
       self.velocity = (velocityTarget - projectOnConnection) / (1 + damper * Time.fixedDeltaTime);
   }
}
