using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public EnemyFollow(UnityEngine.Transform target, UnityEngine.Transform me, float maxVelocity,  LayerMask wallLayer, float viewRadius, float maxForce)
    {
       
        _maxVelocity = maxVelocity;
        _wallLayer = wallLayer;
        _target = target;
        _transform = me;
        _viewRadius = viewRadius;
        _maxForce = maxForce;
    }
    

    public void OnEnter() 
    { 
      Debug.Log("EnemyFollow");

    }

    public void OnExit() { }

    public void OnUpdate()
    {
       
        Console.WriteLine("EnemyFollow");
       FollowLeader(_target, _transform);




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
        if (dist > _viewRadius) return Seek(targetPos);

        return Seek(targetPos, _maxVelocity * (dist / _viewRadius));
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

}