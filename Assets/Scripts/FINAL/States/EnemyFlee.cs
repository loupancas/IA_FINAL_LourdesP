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
    private Queue<Vector3> pathQueue;
    private TP2_Manager_ProfeAestrella _pathfindingManager;
    float _velocity;
    public Node_Script_OP2 NearesHomwNode;

    public EnemyFlee(Transform target, Transform me, float velocity, LayerMask layerMask, TP2_Manager_ProfeAestrella pathfindingManager, Node_Script_OP2 node)
    {
      
        _me = me;
        _maskObstacle = layerMask;
        _target = target;
        _pathfindingManager = pathfindingManager;
        _velocity = velocity > 0 ? velocity : 1.0f;
        NearesHomwNode = node;
         pathQueue = new Queue<Vector3>();

    }

    public void OnEnter() 
    {
        Debug.Log("Flee");
        //NearesHomwNode = FindNearestNode();
        FleeTime(_target);

    }

    public void OnExit() { }

    public void OnUpdate()
    {
     
        Console.WriteLine("EnemyFlee");
     
        if (pathQueue.Count > 0)
        {
            MoveAlongPath();
         
        }

    }



    void FleeTime(Transform targetPosition)
    {
        // Obtener los nodos inicial y final
        Node_Script_OP2 startNode = FindNearestNode();
        Debug.Log("StartNode: " + startNode);
        Node_Script_OP2 endNode = _pathfindingManager.FindNodeNearPoint(targetPosition.position);
        Debug.Log("EndNode: " + endNode);

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
        Debug.Log("Camino calculado:" + pathQueue.Count);
     
        //isMoving = true;
        
        foreach (var pos in pathQueue)
        {
            Debug.Log("Nodo en pathQueue: " + pos);
        }

      

    }
    void MoveAlongPath()
    {
        if (pathQueue.Count == 0)
            return;
        Vector3 targetPos = pathQueue.Peek();
        float distanceToNode = Vector3.Distance(_me.position, targetPos);
        Debug.Log($"Distancia: {distanceToNode}");
        

        if (Vector3.Distance(_me.position, targetPos) > 0.2f)
        {
            Vector3 moveDirection = (targetPos - _me.position).normalized;
           _me .position += moveDirection * _velocity * Time.deltaTime;
            _me.up = moveDirection;
            Debug.Log("Moving");
        }
        else
        {
            pathQueue.Dequeue();
            //Debug.Log("Reached waypoint, dequeuing next point.");
        }



      
    }

  

  


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

}