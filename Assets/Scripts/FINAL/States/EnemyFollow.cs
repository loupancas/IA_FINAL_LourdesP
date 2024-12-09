using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyFollow : IState
{
    
    UnityEngine.Transform _transform;
    float _maxVelocity;
    Vector3 _velocity;
    UnityEngine.Transform _target;
    LayerMask _wallLayer;
    float _viewRadius;
    float _maxForce;
    bool _evade;
    LayerMask _obstacle;
    float _separationWeight;
    public EnemyFollow(UnityEngine.Transform target, UnityEngine.Transform me, float maxVelocity,  LayerMask wallLayer, float viewRadius, float maxForce, LayerMask obstacle, bool evade, float separation)
    {
       
        _maxVelocity = maxVelocity;
        _wallLayer = wallLayer;
        _target = target;
        _transform = me;
        _viewRadius = viewRadius;
        _maxForce = maxForce;
        _evade = evade;
        _obstacle = obstacle;
        _separationWeight = separation;
    }
    

    public void OnEnter() 
    { 
      //Debug.Log("EnemyFollow");

    }

    public void OnExit() { }

    public void OnUpdate()
    {

        Vector3 avoidanceForce = ObstacleAvoidance();
        if (avoidanceForce != Vector3.zero)
        {
            AddForce(avoidanceForce);
        }
        else
        {
            FollowLeader(_target, _transform);
        }

        _transform.position += _velocity * Time.deltaTime;


    }


    void FollowLeader(UnityEngine.Transform _Leader, UnityEngine.Transform me)
    {
        if (Vector3.Distance(me.position, _Leader.position) > 1f)
        {
            Vector3 moveDirection = (_Leader.position - me.position).normalized;
           
            AddForce(moveDirection * _maxVelocity);
            Flocking();

        }
        else
        {
            AddForce(Arrive(_Leader.position));
        }

    }

    private void Flocking()
    {
        var boids = GameManager.instance.allAgents;
        AddForce(Separation(boids) * _separationWeight);
    }

    protected Vector3 Separation(List<EnemigoBase> agents)
    {
        Vector3 desired = Vector3.zero;

        foreach (var item in agents)
        {
            if (item == null || item.transform == _transform) continue;

            Vector3 toAgent = _transform.position - item.transform.position;
            float distSqr = toAgent.sqrMagnitude;

            if (distSqr < _viewRadius * _viewRadius && distSqr > 0)
            {
              
                desired += toAgent.normalized / Mathf.Sqrt(distSqr);
            }
        }

        if (desired == Vector3.zero) return Vector3.zero;
        desired.Normalize();
        desired *= _maxVelocity;

        return CalculateSteering(desired);
    }

    protected Vector3 CalculateSteering(Vector3 desired)
    {
        Vector3 steering = desired - _velocity;
        return Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);
    }

    private void AddForce(Vector3 force)
    {
        _velocity += force;
        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);
    }


    private Vector3 Arrive(Vector3 targetPos)
    {
        float distance = Vector3.Distance(_transform.position, targetPos);
        float stopRadius = 2f;

        if (distance > _viewRadius)
        {
            return Seek(targetPos);
        }

        if (distance < stopRadius)
        {
            return Vector3.zero;
        }

        float speed = _maxVelocity * (distance / (_viewRadius + 5f));
        return Seek(targetPos, speed);
    }


    protected Vector3 Seek(Vector3 targetPos, float speed)
    {
        Vector3 desired = (targetPos - _transform.position).normalized * speed;
        return CalculateSteering(desired);
    }

    protected Vector3 Seek(Vector3 targetPos)
    {
        return Seek(targetPos, _maxVelocity);
    }

    protected Vector3 ObstacleAvoidance()
    {
        Vector3 leftRay = _transform.position - _transform.up * 0.5f;
        Vector3 rightRay = _transform.position + _transform.up * 0.5f;

        if (Physics2D.Raycast(rightRay, _transform.right, _viewRadius, _obstacle))
        {
            return Seek(_transform.position - _transform.up);
        }
        else if (Physics2D.Raycast(leftRay, _transform.right, _viewRadius, _obstacle))
        {
            return Seek(_transform.position + _transform.up);
        }

        return Vector3.zero;
    }

  


}