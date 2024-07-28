
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLostView : IState
{
    FSM _fsm;
    Transform _transform;
    LayerMask _maskPlayer;
    TeamFlockingBase _me;
    float _viewRadius;
    float _viewAngle;

    public EnemyLostView(FSM fSM, Transform transform, LayerMask layerMask, float viewRadius, float viewAngle)
    {
        _fsm = fSM;
        _transform = transform;
        _maskPlayer = layerMask;
        _viewAngle = viewAngle;
        _viewRadius = viewRadius;
       
    }

    public void OnEnter()
    {

    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        var targetLeader = GameManager.instance.GetLeader(GameManager.instance.GetOppositeTeam(_me.Team));

        if (InFOV(targetLeader.transform))
        {
            _fsm.ChangeState("Attack");
        }



    }

    protected bool InLineOfSight(Vector3 start, Vector3 end)
    {
        var dir = end - start;

        return !Physics.Raycast(start, dir, dir.magnitude, _maskPlayer);
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
}

