using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyFlee : IState
{
    Transform _transform;
    Transform _target;
    LayerMask _maskObstacle;
    private Queue<Vector3> pathQueue;
  
    private TP2_Manager_ProfeAestrella _pathfindingManager;
    float _velocity;
    public EnemyFlee(Transform transform, Transform target, float velocity, LayerMask layerMask, TP2_Manager_ProfeAestrella pathfindingManager)
    {
      
        _transform = transform;
        _maskObstacle = layerMask;
        _target = target;
        _pathfindingManager = pathfindingManager;
        _velocity = velocity;
    }

    public void OnEnter() 
    {
        Debug.Log("EnemyFlee");
    }

    public void OnExit() { }

    public void OnUpdate()
    {
        Console.WriteLine("EnemyFlee");
        FleeTime(_transform);
        MoveAlongPath();
    }

  

    void FleeTime(Transform targetPosition)
    {
        // Obtener los nodos inicial y final
        Node_Script_OP2 startNode = _pathfindingManager.FindNodeNearPoint(_transform.position);
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
        if (Vector3.Distance(_transform.position, targetPos) > 0.1f)
        {
            Vector3 moveDirection = (targetPos - _transform.position).normalized;
            _transform.position += moveDirection * _velocity * Time.deltaTime;
            _transform.forward = moveDirection;
        }
        else
        {
            pathQueue.Dequeue();
        }

    }
}