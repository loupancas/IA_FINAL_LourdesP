﻿using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    public EnemyFollow(UnityEngine.Transform target, UnityEngine.Transform me, float maxVelocity,  LayerMask wallLayer, float viewRadius, float maxForce, LayerMask obstacle, bool evade)
    {
       
        _maxVelocity = maxVelocity;
        _wallLayer = wallLayer;
        _target = target;
        _transform = me;
        _viewRadius = viewRadius;
        _maxForce = maxForce;
        _evade = evade;
        _obstacle = obstacle;
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

        


    }


    void FollowLeader(UnityEngine.Transform _Leader, UnityEngine.Transform me)
    {
        if (Vector3.Distance(me.position, _Leader.position) > 1f)
        {
            Vector3 moveDirection = (_Leader.position - me.position).normalized;
            me.position += moveDirection * _maxVelocity * Time.deltaTime;
          
        }
        else
        {
            AddForce(Arrive(_Leader.position));
        }

    }

  

    void AddForce(Vector3 dir)
    {
        _velocity += dir;

        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);
    }


    protected Vector3 Arrive(Vector3 targetPos)
    {
        float dist = Vector3.Distance(_transform.position, targetPos);
        float stopRadius = 1.0f;
        if (dist > _viewRadius) return Seek(targetPos);
        if (dist < stopRadius)
            return Vector3.zero;

        return Seek(targetPos, _maxVelocity * (dist / (_viewRadius+5f)));
    }

    protected Vector3 Seek(Vector3 targetPos, float speed)
    {
        Vector3 desired = (targetPos - _transform.position).normalized * speed;
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);
        return steering;
    }

    protected Vector3 Seek(Vector3 targetPos)
    {
        return Seek(targetPos, _maxVelocity);
    }

    protected Vector3 ObstacleAvoidance()
    {
        if (Physics2D.Raycast(_transform.position + _transform.up * 0.5f, _transform.right, _viewRadius, _obstacle))
        {
            _evade = true;
            Debug.Log("Evade");
            return Seek(_transform.position - _transform.up);
        }
        else if (Physics2D.Raycast(_transform.position - _transform.up * 0.5f, _transform.right, _viewRadius, _obstacle))
        {
           _evade = true;
            Debug.Log("Evade");
            return Seek(_transform.position + _transform.up);
        }

        _evade = false;
        return Vector3.zero;
    }

}