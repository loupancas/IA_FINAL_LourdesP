using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyMovement : IState
{
    FSM _fsm;
    TeamFlockingBase _me;
    Transform _transform;
    List<TeamFlockingBase> _boids;
    float _maxVelocity;
    float _maxForce;
    float _viewRadius;
    float _viewAngle;
    Vector3 _velocity;
    LayerMask _wallLayer;

    private Queue<Vector3> pathQueue;
    private TP2_Manager_ProfeAestrella _pathfindingManager;

    public EnemyMovement(FSM fsm, float maxVelocity, float maxForce, float viewRadius, float viewAngle, LayerMask wallLayer, TeamFlockingBase me, List<TeamFlockingBase> boids, TP2_Manager_ProfeAestrella pathfindingManager)
    {
        _me = me;
        _fsm = fsm;
        _transform = me.transform;
        _maxVelocity = maxVelocity;
        _maxForce = maxForce;
        _viewRadius = viewRadius;
        _viewAngle = viewAngle;
        _wallLayer = wallLayer;
        _boids = boids;

        pathQueue = new Queue<Vector3>();
        _pathfindingManager = pathfindingManager;
    }

    public void OnEnter()
    {
        Debug.Log("Entered Movement State");
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        var targetLeader = GameManager.instance._pinkLeader.transform;
        var otherTeamMembers = GameManager.instance.GetOppositeTeam(_me.Team);

         if(Vector3.Magnitude(_transform.position-targetLeader.position)>_viewRadius)
         {
            // Si el líder no está en el FOV, buscarlo
            CalculatePath(targetLeader.position);
            MoveAlongPath();
         }


       
        

    }

    public void SearchTime()
    {
        var targetLeader = GameManager.instance._pinkLeader.transform;
        //if (Vector3.Magnitude(transform.position - targetLeader.position) > viewRadius)
        //{
        //    CalculatePath(targetLeader.position);
        //    MoveAlongPath();
        //}
    }


    void AddForce(Vector3 dir)
    {
        _velocity += dir;

        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);
    }
    Vector3 Seek(Vector3 dir)
    {
        var desired = dir - _transform.position;
        desired.Normalize();
        desired *= _maxVelocity;

        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);

        return steering;
    }

    void CalculatePath(Vector3 targetPosition)
    {
        // Obtener los nodos inicial y final
        Node_Script_OP2 startNode = _pathfindingManager.FindNodeNearPoint(_transform.position);
        Node_Script_OP2 endNode = _pathfindingManager.FindNodeNearPoint(targetPosition);

        // Calcular el camino con Theta*
        _pathfindingManager.PathFinding(_pathfindingManager._Path, startNode, endNode, _wallLayer);

        // Convertir el camino a una cola de posiciones
        pathQueue = new Queue<Vector3>(_pathfindingManager._Path.Select(node => node.position));
        //pathQueue.Clear();

        foreach (var node in _pathfindingManager._Path)
        {
            Debug.DrawLine(node.position, node.position + Vector3.up * 2, Color.red, 2.0f);
        }
    }

    void MoveAlongPath()
    {
        if (pathQueue.Count == 0)
            return;

        Vector3 targetPos = pathQueue.Peek();
        if (Vector3.Distance(_transform.position, targetPos) > 0.1f)
        {
            Vector3 moveDirection = (targetPos - _transform.position).normalized;
            _transform.position += moveDirection * _maxVelocity * Time.deltaTime;
            _transform.forward = moveDirection;
        }
        else
        {
            pathQueue.Dequeue();
        }
    }



    protected bool InLineOfSight(Vector3 start, Vector3 end)
    {
        var dir = end - start;

        return !Physics.Raycast(start, dir, dir.magnitude, _wallLayer);

    }

    public bool InFOV(Transform obj)
    {
        var dir = obj.position - _transform.position;

        if (dir.magnitude < _viewRadius)
        {
            if (Vector3.Angle(_transform.forward, dir) <= _viewAngle * 0.5f)
            {
                return InLineOfSight(_transform.position, obj.position);
            }
        }

        return false;
    }

    Vector3 Separation(List<TeamFlockingBase> boids, float radius)
    {
        Vector3 desired = Vector3.zero;

        foreach (var item in boids)
        {
            var dir = item.transform.position - _transform.position;
            if (dir.magnitude > radius || item == _me)
                continue;

            desired -= dir;
        }

        if (desired == Vector3.zero)
            return desired;

        desired.Normalize();
        desired *= _maxVelocity;

        return CalculateSteering(desired);
    }

    Vector3 CalculateSteering(Vector3 desired)
    {
        var steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce);

        return steering;
    }
}