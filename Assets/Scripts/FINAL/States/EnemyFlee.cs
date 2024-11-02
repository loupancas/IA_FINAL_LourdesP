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
    private bool isMoving;
    private TP2_Manager_ProfeAestrella _pathfindingManager;
    float _velocity;
    public Node_Script_OP2 NearestEnemyNode;
    private float _timeSinceLastUpdate = 0f;
    private float _updateInterval = 0.25f;

    public EnemyFlee(Transform target, Transform me, float velocity, LayerMask layerMask, TP2_Manager_ProfeAestrella pathfindingManager, Node_Script_OP2 node)
    {
      
        _me = me;
        _maskObstacle = layerMask;
        _target = target;
        _pathfindingManager = pathfindingManager;
        _velocity = velocity > 0 ? velocity : 1.0f;
        NearestEnemyNode = node;
         pathQueue = new Queue<Vector3>();

    }

    public void OnEnter() 
    {
        Debug.Log("Flee");
        NearestEnemyNode = FindNearestNode();
        //FleeTime(_target);

    }

    public void OnExit() { }

    public void OnUpdate()
    {
        _timeSinceLastUpdate += Time.deltaTime;
        if (_timeSinceLastUpdate >= _updateInterval)
        {
            NearestEnemyNode = FindNearestNode();
            _timeSinceLastUpdate = 0f;
        }

        FleeTime(_target);
        Console.WriteLine("EnemyFlee");
        if (isMoving && pathQueue.Count > 0)
        {
            MoveAlongPath();
            isMoving = false;
        }

        _pathfindingManager._NearestEnemyNode = NearestEnemyNode;

    }

  

    void FleeTime(Transform targetPosition)
    {
        // Obtener los nodos inicial y final
        Node_Script_OP2 startNode =NearestEnemyNode;
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
        Debug.Log("Camino calculado:");
     
        isMoving = true;
        
        foreach (var pos in pathQueue)
        {
            Debug.Log("Nodo en pathQueue: " + pos);
        }

        //pathQueue.Clear();
        //MoveAlongPath();

    }
    void MoveAlongPath()
    {
        if (pathQueue.Count == 0)
            return;
        Vector3 targetPos = pathQueue.Peek();
        float distanceToNode = Vector3.Distance(_me.position, targetPos);
        Debug.Log($"Distancia: {distanceToNode}");
        //Debug.Log("Posición actual: " + _me.position + ", Nodo objetivo: " + targetPos + ", Distancia al nodo: " + distanceToTarget);
        //if (distanceToTarget > 0.1f)
        //{
        //    //Vector3 moveDirection = (targetPos - _me.position).normalized;
        //    //_me.position += moveDirection * _velocity * Time.deltaTime;
        //    //_me.forward = moveDirection;
        //    //Debug.Log("Moving along path");

        //    _me.position = Vector3.MoveTowards(_me.position, targetPos, _velocity * Time.deltaTime);
        //    _me.forward = (targetPos - _me.position).normalized;
        //    Debug.Log("Moving along path");


        //}
        //else
        //{
        //    pathQueue.Dequeue();
        //    Debug.Log("Reached a waypoint, moving to the next one");
        //}
        if (distanceToNode < 0.2f)
        {
            // Si está cerca, retira el nodo alcanzado y pasa al siguiente
            pathQueue.Dequeue();
            if (pathQueue.Count > 0)
            {
                targetPos = pathQueue.Peek(); // Nuevo objetivo
                Debug.Log("nuevo objetivo");
            }
            else
            {
                Debug.Log("Destination reached");
                // Puedes agregar un cambio de estado o cualquier otra acción al llegar
                return;
            }
        }

        // Movimiento hacia el nodo objetivo
        Vector3 direction = (targetPos - _me.position).normalized;
        _me.position += direction * (_velocity * Time.deltaTime);
        _me.up = direction;
        Debug.Log($"Moviendo hacia el nodo: {targetPos} ");
    }

    IEnumerator CorutineFindNearestNode()
    {
        float Delay = 0.25f;
        WaitForSeconds wait = new WaitForSeconds(Delay);

        while (true)
        {
            yield return wait;
            NearestEnemyNode = FindNearestNode();
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