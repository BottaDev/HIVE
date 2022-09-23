using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamTesting : MonoBehaviour
{
    [SerializeField] Vector3 _tangent = Vector3.one;// this is your "predetermined vector"
    [SerializeField] Vector3 _approxNormal = Vector3.up;// an additional normal to help define a plane
    void OnDrawGizmos ()
    {
        Vector3 betterNormal = Vector3.Cross( _tangent.normalized , -Vector3.Cross(_tangent.normalized,_approxNormal) );
        Vector3 X0Z = Quaternion.Euler(0,((System.DateTime.Now.Millisecond % 1000)/1000f)*360f,0) * Vector3.right;
         
        // option 1, point above X0Z that lies on a plane:
        new Plane{ normal=betterNormal }.Raycast( new Ray{ origin=X0Z , direction=Vector3.up } , out float Y );
        Vector3 XYZ = new Vector3( X0Z.x , Y , X0Z.z );
 
        // option 2, nearest point on a plane:
        // Vector3 XYZ = Vector3.ProjectOnPlane( vector:X0Z , planeNormal:betterNormal );
 
        Vector3 pos = transform.position;
        Gizmos.color = Color.magenta; Gizmos.DrawLine( pos , pos + _tangent );
        Gizmos.color = Color.cyan * 0.5f; Gizmos.DrawLine( pos , pos + _approxNormal );
        Gizmos.color = Color.cyan; Gizmos.DrawLine( pos , pos + betterNormal );
        Gizmos.color = Color.green * 0.5f; Gizmos.DrawLine( pos , pos + X0Z );
        Gizmos.color = Color.green; Gizmos.DrawLine( pos , pos + XYZ );
        Gizmos.matrix = Matrix4x4.Rotate( Quaternion.LookRotation(_tangent,betterNormal) );
        Gizmos.color = new Color( 0.1f , 1f , 0.1f , 0.1f );
        Gizmos.DrawCube( pos , new Vector3{x=3,z=3} );
    }
    
}
