﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySearch : IState
{
    
    Transform _transform;
    float _maxVelocity;
    float _viewRadius;
    Vector3 _velocity;
    Transform _target;
    LayerMask _wallLayer;
    LayerMask _obstacle;
    private Queue<Vector3> pathQueue;
    private TP2_Manager_ProfeAestrella _pathfindingManager;
    public Node_Script_OP2 NearestNode;
    bool _evade;
    public EnemySearch(Transform target, Transform me, float maxVelocity,  LayerMask wallLayer, TP2_Manager_ProfeAestrella pathfindingManager, Node_Script_OP2 node, LayerMask obstacle, float viewRadius, bool evade)
    {

        _maxVelocity = maxVelocity; 
        _wallLayer = wallLayer;
        _obstacle = obstacle;
        _pathfindingManager = pathfindingManager;
        _target = target;
        NearestNode = node;
        _transform = me;
        pathQueue = new Queue<Vector3>();
        _viewRadius = viewRadius;
        _evade = evade;
    }
    

    public void OnEnter() 
    { 
        CalculatePath(_target);

    }

    public void OnExit() { }

    public void OnUpdate()
    {

        if (pathQueue.Count > 0)
        {
            MoveAlongPath();

        }





    }

    void AddForce(Vector3 dir)
    {
        _velocity += dir;

        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);
    }
   

    void CalculatePath(Transform targetPosition)
    {
        Node_Script_OP2 startNode =_pathfindingManager.FindNodeNearPoint(_transform.position);
        Debug.Log("StartNode: " + startNode);
        Node_Script_OP2 endNode =_pathfindingManager.FindNodeNearPoint(targetPosition.position);
        Debug.Log("EndNode: " + endNode);
        List<Transform> path = _pathfindingManager.CalculatePath(startNode, endNode, _wallLayer);


        pathQueue = new Queue<Vector3>(path.Select(node => node.position));

       

    }

    protected Vector3 ObstacleAvoidance()
    {
        Vector3 avoidanceForce = Vector3.zero;

        float[] angles = { -45, -30, -15, 0, 15, 30, 45 };
        foreach (float angle in angles)
        {
            Vector3 direction = Quaternion.Euler(0, 0, angle) * _transform.right;

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(_transform.position, direction, out hit, _viewRadius, _obstacle))
            {
                Vector3 awayFromObstacle = (_transform.position - hit.point).normalized;
                avoidanceForce += awayFromObstacle;
            }


        }

        if (avoidanceForce != Vector3.zero)
        {
            _evade = true;
            Debug.Log("Evade");
            return avoidanceForce.normalized * _maxVelocity;
        }

        _evade = false;
        return Vector3.zero;
    }


    void MoveAlongPath()
    {
        Vector3 avoidanceForce = ObstacleAvoidance();

           

        if (pathQueue.Count == 0)
        {
            return;
        }

        Vector3 targetPos = pathQueue.Peek();
        if (Vector3.Distance(_transform.position, targetPos) <= 2.5f)
        {

            pathQueue.Dequeue();
           
        }
        else
        {
            Vector3 moveDirection = (targetPos - _transform.position).normalized;
            Debug.Log($"Distancia al nodo objetivo: {Vector3.Distance(_transform.position, targetPos)}");

            if (avoidanceForce != Vector3.zero)
            {
                               
                 avoidanceForce += moveDirection * 0.5f;                
                 AddForce(avoidanceForce);
                _transform.position += _velocity * Time.deltaTime;
                return;
            }

            _transform.position += moveDirection * _maxVelocity * Time.deltaTime;


        }
    }


  

}