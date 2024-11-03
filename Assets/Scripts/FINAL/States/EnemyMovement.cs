using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyMovement : IState
{
    
    Transform _transform;
    float _maxVelocity;
    Vector3 _velocity;
    Transform _target;
    LayerMask _wallLayer;
    private Queue<Vector3> pathQueue;
    private TP2_Manager_ProfeAestrella _pathfindingManager;
    public Node_Script_OP2 NearestNode;

    public EnemyMovement(Transform target, Transform me, float maxVelocity,  LayerMask wallLayer, TP2_Manager_ProfeAestrella pathfindingManager, Node_Script_OP2 node)
    {
       
        _maxVelocity = maxVelocity;
        _wallLayer = wallLayer;
        _pathfindingManager = pathfindingManager;
        _target = target;
        NearestNode = node;
        _transform = me;
        pathQueue = new Queue<Vector3>();
    }
    

    public void OnEnter() 
    { 
      //Debug.Log("EnemyMovement");
        CalculatePath(_target);

    }

    public void OnExit() { }

    public void OnUpdate()
    {
       
        //Console.WriteLine("EnemyMovement");
        MoveAlongPath();
        


        
    }

    void AddForce(Vector3 dir)
    {
        _velocity += dir;

        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);
    }
   

    void CalculatePath(Transform targetPosition)
    {
        // Obtener los nodos inicial y final
        Node_Script_OP2 startNode =_pathfindingManager.FindNodeNearPoint(_transform.position);
        Node_Script_OP2 endNode =_pathfindingManager.FindNodeNearPoint(targetPosition.position);

        // Calcular el camino con Theta*
        //_pathfindingManager.PathFinding(_pathfindingManager._Path, startNode, endNode, _wallLayer);
        List<Transform> path = _pathfindingManager.CalculatePath(startNode, endNode, _wallLayer);


        // Convertir el camino a una cola de posiciones
        pathQueue = new Queue<Vector3>(path.Select(node => node.position));
        //pathQueue.Clear();

        //Debug.Log("Path Calculated" + targetPosition);

    }

    void MoveAlongPath()
    {
        if (pathQueue.Count == 0)
        {

            //Debug.Log("PathQueue is empty");

            return;
        }
        Vector3 targetPos = pathQueue.Peek();
        //Debug.Log("Moving to next waypoint");
        if (Vector3.Distance(_transform.position, targetPos) > 0.1f)
        {
            Vector3 moveDirection = (targetPos - _transform.position).normalized;
            _transform.position += moveDirection * _maxVelocity * Time.deltaTime;
            _transform.up = moveDirection;
            //Debug.Log("Moving");
        }
        else
        {
            pathQueue.Dequeue();
            //Debug.Log("Reached waypoint, dequeuing next point.");
        }

    }

   

}