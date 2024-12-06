using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.EventSystems;

public class EnemyFlee : IState
{
    Transform _me;
    Transform _target;
    LayerMask _maskObstacle;
    LayerMask _obstacle;
    private Queue<Vector3> pathQueue;
    private TP2_Manager_ProfeAestrella _pathfindingManager;
    Vector3 _velocity;
    float _maxVelocity;
    public Node_Script_OP2 NearesHomwNode;
    public bool _evade;
    public float _viewRadius;

    public EnemyFlee(Transform target, Transform me, float velocity, LayerMask layerMask, TP2_Manager_ProfeAestrella pathfindingManager, Node_Script_OP2 node, LayerMask obstacle, float viewRadius, bool evade)
    {
      
        _me = me;
        _maskObstacle = layerMask;
        _target = target;
        _pathfindingManager = pathfindingManager;
        _maxVelocity = velocity > 0 ? velocity : 1.0f;
        NearesHomwNode = node;
         pathQueue = new Queue<Vector3>();
        _obstacle = obstacle;
        _viewRadius = viewRadius;
        _evade = evade;

    }

    public void OnEnter() 
    {
        FleeTime(_target);

    }

    public void OnExit() { }

    public void OnUpdate()
    {
     
     
        if (pathQueue.Count > 0)
        {
            MoveAlongPath();
         
        }

    }



    void FleeTime(Transform targetPosition)
    {
        Node_Script_OP2 startNode = FindNearestNode();
        Node_Script_OP2 endNode = _pathfindingManager.FindNodeNearPoint(targetPosition.position);

        if (startNode == null )
        {
            Debug.LogError("StartNode is null.");
            return;
        }
        if (endNode == null)
        {
            Debug.LogError("EndNode is null.");
            return;
        }

        List<Transform> path = _pathfindingManager.CalculatePath(startNode, endNode, _maskObstacle);
        if (path == null || path.Count == 0)
        {
            Debug.LogError("El camino calculado está vacío o es nulo.");
            return;
        }
        pathQueue = new Queue<Vector3>(path.Select(node => node.position));
     
        
        foreach (var pos in pathQueue)
        {
            Debug.Log("Nodo en pathQueue: " + pos);
        }

      

    }
    //void MoveAlongPath()
    //{
    //    if (pathQueue.Count == 0)
    //        return;
    //    Vector3 targetPos = pathQueue.Peek();
    //    float distanceToNode = Vector3.Distance(_me.position, targetPos);
        

    //    if (Vector3.Distance(_me.position, targetPos) > 0.2f)
    //    {
    //        Vector3 moveDirection = (targetPos - _me.position).normalized;
    //       _me .position += moveDirection * _velocity * Time.deltaTime;
    //        _me.up = moveDirection;
    //    }
    //    else
    //    {
    //        pathQueue.Dequeue();
    //    }



      
    //}

    private Node_Script_OP2 FindNearestNode()
    {
        Node_Script_OP2 nearest = null;
        float NearestVal = float.MaxValue;
        foreach (Node_Script_OP2 CurrentNode in _pathfindingManager._NodeList)
        {
            float CurrentDis = Vector3.Distance(CurrentNode.NodeTransform.position, _me.position);
            if (CurrentDis < NearestVal)
            {
                NearestVal = CurrentDis;
                nearest = CurrentNode;
            }
        }
        return nearest;
    }

    void AddForce(Vector3 dir)
    {
        _velocity += dir;

        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);
    }

    void MoveAlongPath()
    {
        Vector3 avoidanceForce = ObstacleAvoidance();

        if (avoidanceForce != Vector3.zero)
        {
            if (pathQueue.Count > 0)
            {
                Vector3 targetPosS = pathQueue.Peek();
                Vector3 moveDirection = (targetPosS - _me.position).normalized;
                avoidanceForce += moveDirection * 0.5f;
            }

            AddForce(avoidanceForce);
            _me.position += _velocity * Time.deltaTime;
            return;
        }

        if (pathQueue.Count == 0)
        {
            return;
        }

        Vector3 targetPos = pathQueue.Peek();
        if (Vector3.Distance(_me.position, targetPos) > 0.1f)
        {
            Vector3 moveDirection = (targetPos - _me.position).normalized;
            _me.position += moveDirection * _maxVelocity * Time.deltaTime;
            _me.up = moveDirection;
        }
        else
        {
            pathQueue.Dequeue();
        }
    }

    protected Vector3 ObstacleAvoidance()
    {
        Vector3 avoidanceForce = Vector3.zero;

        float[] angles = { -45, -30, -15, 0, 15, 30, 45 };
        foreach (float angle in angles)
        {
            Vector3 direction = Quaternion.Euler(0, 0, angle) * _me.right;

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(_me.position, direction, out hit, _viewRadius, _obstacle))
            {
                Vector3 awayFromObstacle = (_me.position - hit.point).normalized;
                avoidanceForce += awayFromObstacle;
            }


        }

        if (avoidanceForce != Vector3.zero)
        {
            _evade = true;
            return avoidanceForce.normalized * _maxVelocity;
        }

        _evade = false;
        return Vector3.zero;
    }




}