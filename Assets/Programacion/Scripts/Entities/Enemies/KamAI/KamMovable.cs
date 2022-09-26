using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

//This is just a new ground-up version of AI made by another member of the team.
public class KamMovable : MonoBehaviour
{
    [Header("Ground")]
    public Transform floorPivot;
    public Transform visionPoint;
    public float floorDistanceCheck = 0.1f;
    public LayerMask floorMask;

    [Header("Movement")]
    public Vector3 _velocity;
    public Transform followPoint;
    private Vector3 lastSavedPosition;

    public float stopAtDistance;
    public float scaredDistance;
    
    public List<KamNode> path;
    public int currentNode;
    public bool canCalculateNewPath = true;
    public bool setRotation = true;
    
    public bool paused => UIPauseMenu.paused;

    [Header("Events")]
    public UnityEvent onFollowing;
    public UnityEvent onIdle;
    public UnityEvent onScared;

    private void Start()
    {
        EventManager.Instance.Subscribe("SendPlayerReference", GetPlayerReference);
        EventManager.Instance.Trigger("NeedsPlayerReference");
        EventManager.Instance.Unsubscribe("SendPlayerReference", GetPlayerReference);
        
        SnapToGround(10f);
        lastSavedPosition = followPoint.position;
        KamMovableManager.i.Add(this);
    }
    
    private void GetPlayerReference(params object[] p)
    {
        followPoint = ((Player)p[0]).transform;
    }

    private RaycastHit hit;
    void SnapToGround(float distanceCheck)
    {
        Ray ray = new Ray(floorPivot.position, -floorPivot.up);
        if (Physics.Raycast(ray, out hit, distanceCheck, floorMask))
        {
            floorPivot.position = hit.point;
            floorPivot.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            
            floorPivot.position += floorPivot.up * 0.1f;
        }
        else
        {
            //There is no floor to snap to, AKA you're falling
            //SteeringBehaviors.ApplyForce(ref _velocity, Physics.gravity, KamMovableManager.i.maxSpeed);

            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, 0.1f * Time.deltaTime);
        }
    }

    private void Update()
    {
        if (paused) return;
        
        //Separate from all boids
        Vector3 separation = SteeringBehaviors.Separation(gameObject, 
            KamMovableManager.i.allBoids.Where(x => Vector3.Distance(transform.position, x.transform.position) < KamMovableManager.i.separationDistance && x != this)
                .Select(x => x.gameObject)
                .ToList()
            , _velocity
            , KamMovableManager.i.maxSpeed
            , KamMovableManager.i.maxForce
            , KamMovableManager.i.separationWeight);

        if(separation != Vector3.zero)
        {
            //separate
            SteeringBehaviors.ApplyForce(ref _velocity, separation, KamMovableManager.i.maxSpeed);
            Move(ref _velocity);
        }
        else
        {
            if (Vector3.Distance(followPoint.position, transform.position) > stopAtDistance)
            {
                MoveTowards(followPoint.position);
                
                onFollowing?.Invoke();
            }
            else if (Vector3.Distance(followPoint.position, transform.position) < scaredDistance)
            {
                Vector3 awayFromPlayer = (transform.position - followPoint.position).normalized;
                MoveTowards(transform.position + awayFromPlayer, delegate {  },false);
                
                Debug.DrawLine(transform.position, transform.position + awayFromPlayer);
                
                onScared?.Invoke();
            }
            else
            {
                _velocity = Vector3.Lerp(_velocity, Vector3.zero, 7 * Time.deltaTime);
                Move(ref _velocity);
                
                onIdle?.Invoke();
            }
        }
        
    }

    public void Move(ref Vector3 _velocity)
    {
        _velocity.y = 0;
        Vector3 velocity = (_velocity * Time.deltaTime);
        Vector3 newPos = transform.position + velocity;

        if (setRotation && velocity.magnitude > 0.001f)
        {
            //Vector3 normal = transform.up;
            transform.forward = velocity;
            //transform.forward = Vector3.ProjectOnPlane(transform.forward, normal);
        }
        
        PathfindingBehaviors.Move(gameObject, newPos, floorMask);
        SnapToGround(floorDistanceCheck);
        
        
        Debug.DrawLine(transform.position, transform.position + velocity, Color.black);
        
        Debug.DrawLine(transform.position, 
            transform.position + velocity.normalized
            , Color.magenta);
    }
    
    public void MoveTowards(Vector3 pos, Action onArrive = null, bool mustBeInSight = true)
    {
        Vector3 dir = pos - transform.position;
        Vector3 newPos = dir.normalized * KamMovableManager.i.maxSpeed * Time.deltaTime;
        newPos.y = transform.position.y;
        bool sight = !mustBeInSight || PathfindingBehaviors.LineOfSight(floorPivot.position, pos, KamMovableManager.i.wallMask);
        if (sight)
        {
            #region Use arrive + obstacle avoidance to move towards the position
            
            Vector3 arrive = SteeringBehaviors.Arrive(
            transform.position,
            pos,
            KamMovableManager.i.arriveRadius,
            _velocity,
            KamMovableManager.i.maxSpeed,
            KamMovableManager.i.maxForce,
            KamMovableManager.i.arriveWeight,
            delegate { onArrive?.Invoke(); }
            );
            
            Vector3 avoidObstacles = KamMovableManager.i.obstacleAvoidance ? 
                SteeringBehaviors.ObstacleAvoidance(
                    transform
                    , transform.forward
                    , KamMovableManager.i.rayDistance
                    , KamMovableManager.i.obstacleMask
                    , _velocity
                    , KamMovableManager.i.maxSpeed
                    , KamMovableManager.i.maxForce
                    , "Wall") 
                : Vector3.zero;

            Vector3 result = avoidObstacles == Vector3.zero ? 
                arrive 
                : SteeringBehaviors.Seek(
                    transform, 
                    avoidObstacles, 
                    _velocity, 
                    KamMovableManager.i.maxSpeed, 
                    KamMovableManager.i.maxForce);
            
            SteeringBehaviors.ApplyForce(ref _velocity, result, KamMovableManager.i.maxSpeed);

            
            Move(ref _velocity);
            #endregion
        }
        else
        {
            if (KamMovableManager.i.thetaStar)
            {
                if (canCalculateNewPath)
                {
                    #region Use theta* to find a better path towards the position

                    List<KamNode> pathToGoal = new List<KamNode>();

                    //This try catch is here because there are cases where there is no way for a path to be made. 
                    //This happens because it can't be seen by you nor any node in the map. 
                    //There are specific corners which can be blocked by the moving obstacles, making them impossible to reach.
                    //In these cases, it will throw an error. So instead, with this, it will set its path to null and simply stay still.
                    try
                    {
                        pathToGoal =
                            PathfindingBehaviors.ThetaStar(transform.position, pos, KamMovableManager.i.wallMask);
                    }
                    catch
                    {
                        //ignore
                    }
                    #endregion

                    #region Correct the path's current node when it changes
                    if (path != null && pathToGoal != null && path.Count > 0 && pathToGoal.Count > 0)
                    {
                        if (path[0] != pathToGoal[0])
                        {
                            currentNode -= path.DifferenceAmount(pathToGoal);

                            if (currentNode < 0)
                            {
                                currentNode = 0;
                            }

                            //#region Now choose the last node you can see as your current node
                            //for (int i = 0; i < path.Count; i++)
                            //{
                            //    Node current = path[i];

                            //    if(!PathfindingBehaviors.LineOfSight(transform.position, current.transform.position, BoidManager.i.obstacleMask))
                            //    {
                            //        break;
                            //    }

                            //    currentNode = i;
                            //}
                            //#endregion
                        }
                    }


                    #endregion

                    //set your path
                    path = pathToGoal;
                    
                    canCalculateNewPath = false;
                    
                    Invoke(nameof(PathCalculationReset),1f);
                }

                if (path != null)
                {
                    if (path.Count != 0)
                    {
                        #region Path Drawing
                        Vector3 last = new Vector3();
                        bool first = true;
                        foreach (KamNode node in path)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                Debug.DrawLine(last, node.transform.position, Color.magenta);
                            }

                            last = node.transform.position;
                        }
                        #endregion

                        #region Traverse the Path
                        currentNode = Mathf.Clamp(currentNode, 0, path.Count - 1);
                        KamNode n = path[currentNode];
                        Vector3 destination = n.transform.position;

                        //Move Towards it
                        MoveTowards(destination,
                            delegate
                            {
                                currentNode++;

                                if (currentNode > path.Count - 1)
                                {
                                    //when you are done, eliminate the path and restart the count
                                    currentNode = 0;
                                }
                            }, false
                        );

                        #endregion
                    }
                }
            }
        }
    }

    void PathCalculationReset()
    {
        canCalculateNewPath = true;
    }
    public Vector3 GetVelocity()
    {
        return _velocity;
    }
}
