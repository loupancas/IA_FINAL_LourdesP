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
        Node_Script_OP2 startNode = _pathfindingManager.FindNodeNearPoint(_me.position);
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

        // Calcular el camino con Theta*
        List<Transform> path = _pathfindingManager.CalculatePath(startNode, endNode, _maskObstacle);
        if (path == null || path.Count == 0)
        {
            Debug.LogError("El camino calculado está vacío o es nulo.");
            return;
        }
        // Convertir el camino a una cola de posiciones
        pathQueue = new Queue<Vector3>(path.Select(node => node.position));
        //pathQueue.Clear();


    }
    void MoveAlongPath()
    {
        if (pathQueue.Count == 0)
            return;
        Vector3 targetPos = pathQueue.Peek();

        if (Vector3.Distance(_me.position, targetPos) > 0.1f)
        {
            Vector3 moveDirection = (targetPos - _me.position).normalized;
            _me.position += moveDirection * _velocity * Time.deltaTime;
            _me.forward = moveDirection;
            Debug.Log("Moving along path");

        }
        else
        {
            pathQueue.Dequeue();
        }

    }




}