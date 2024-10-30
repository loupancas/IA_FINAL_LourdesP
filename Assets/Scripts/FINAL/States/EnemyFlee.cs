using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class EnemyFlee : IState
{
    Transform _me;
    Transform _target;
    LayerMask _maskObstacle;
    private Queue<Vector3> pathQueue;
  
    private TP2_Manager_ProfeAestrella _pathfindingManager;
    float _velocity;
    public Node_Script_OP2 NearestEnemyNode;

    public EnemyFlee(Transform target, Transform me, float velocity, LayerMask layerMask, TP2_Manager_ProfeAestrella pathfindingManager, Node_Script_OP2 node)
    {
      
        _me = me;
        _maskObstacle = layerMask;
        _target = target;
        _pathfindingManager = pathfindingManager;
        _velocity = velocity;
         NearestEnemyNode = node;
         pathQueue = new Queue<Vector3>();

    }

    public void OnEnter() 
    {
        Debug.Log("Flee");
        FleeTime(_target);

    }

    public void OnExit() { }

    public void OnUpdate()
    {
        Console.WriteLine("EnemyFlee");
        MoveAlongPath();
    }

  

    void FleeTime(Transform targetPosition)
    {
        // Obtener los nodos inicial y final
        Node_Script_OP2 startNode = NearestEnemyNode;
        Node_Script_OP2 endNode = _pathfindingManager.FindNodeNearPoint(targetPosition.position);

        // Calcular el camino con Theta*
        List<Transform> path = _pathfindingManager.CalculatePath(startNode, endNode, _maskObstacle);

        // Convertir el camino a una cola de posiciones
        pathQueue = new Queue<Vector3>(path.Select(node => node.position));
        //pathQueue.Clear();


    }
    void MoveAlongPath()
    {
        if (pathQueue.Count == 0)
            return;
        Vector3 targetPos = pathQueue.Peek();
        Debug.Log("Moving along path");

        if (Vector3.Distance(_me.position, targetPos) > 0.1f)
        {
            Vector3 moveDirection = (targetPos - _me.position).normalized;
            _me.position += moveDirection * _velocity * Time.deltaTime;
            _me.forward = moveDirection;
        }
        else
        {
            pathQueue.Dequeue();
        }

    }




}