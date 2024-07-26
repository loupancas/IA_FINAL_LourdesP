using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Tp2Sentinel_StatePatrol_OP2 : State
{
    [Header("References")]
    [SerializeField] TP2_Manager_ProfeAestrella _Manager;
    [SerializeField] Tp2_SentinelStateMachine_OP2 _Tp2StateMachine; // ref. al statemachine
    [SerializeField] GameObject _Tp2SentinelOBJ; // ref. gameobject del sentinel

    [Header("State References")]
    [SerializeField] Tp2_SentinelState_Pursue_OP2 _SentinelPursue;
    [SerializeField] Tp2_SentinelState_Search_OP2 _SentinelAlarm;
    
    
    [Header("Variables")]
    [SerializeField] List<Transform> _PatrolWaypoints = new List<Transform>();
    [SerializeField] List<Transform> _ReturnWaypoints = new List<Transform>();
    [SerializeField] float Speed, ArriveDist; // Velocidad del Sentinel y la distancia maxima para que corra arrive
    [SerializeField] LayerMask _Obstacles;
    [SerializeField] bool Returned;

    private void Start()
    {
        _Tp2StateMachine = GetComponentInParent<Tp2_SentinelStateMachine_OP2>();
        _Manager = _Tp2StateMachine._Manager;
        
    }

     
    public override State RunCurrentState()
    {
        if(_Tp2StateMachine.Enemyspotted == true)
        {
            _Tp2StateMachine.SwitchToNewState(_SentinelPursue);
            return _SentinelPursue;
        }
        else if(_Tp2StateMachine.Alarm == true && _Tp2StateMachine.Enemyspotted == false)
        {
            _SentinelAlarm._PlayerNode = _Tp2StateMachine._PlayernearestNode;
            _SentinelAlarm._SentinelNode = _Tp2StateMachine._SentinelNearestNode;
            _SentinelAlarm.Reset();
            _Tp2StateMachine.SwitchToNewState(_SentinelAlarm);
            return _SentinelAlarm;
        }
        else
        {
            //reset del listado de waypoints
            if (CurrentWaypoint >= _PatrolWaypoints.Count)
            {
                CurrentWaypoint = 0;
            }

            if (!Physics2D.Raycast(_Tp2SentinelOBJ.transform.position, (_PatrolWaypoints[CurrentWaypoint].transform.position - _Tp2SentinelOBJ.transform.position).normalized, Vector3.Distance(_Tp2SentinelOBJ.transform.position, _PatrolWaypoints[CurrentWaypoint].transform.position), _Obstacles))
            {
                // no puede ver el nodo de patrulla
              
                PatrolLogic();
                return this;
            

            }
            else
            {
                //puede ver el nodo de patrulla
                print("return");
                
                ReturnLogic();
                return this;

            }

        }
    }


    int CurrentWaypoint;

    private void PatrolLogic()
    {
        //calculo distancia
        float distance = Vector2.Distance(_PatrolWaypoints[CurrentWaypoint].transform.position, _Tp2SentinelOBJ.transform.position);

        Vector3 Director = (_PatrolWaypoints[CurrentWaypoint].transform.position - _Tp2SentinelOBJ.transform.position) * Speed;

        float DirectorAngle = MathF.Atan2(Director.y, Director.x) * Mathf.Rad2Deg;

        _Tp2SentinelOBJ.transform.position += Vector3.ClampMagnitude(Director, Speed);

        _Tp2SentinelOBJ.transform.rotation = Quaternion.Euler(Vector3.forward * DirectorAngle);

        if(distance < ArriveDist)
        {
            CurrentWaypoint++;
        }
    }

    int CurrentReturnWaypoint;
    private void ReturnLogic()
    {
        if (_ReturnWaypoints.Count <= 0)
        {
            
            _Manager.PathFinding(_ReturnWaypoints, _Tp2StateMachine._SentinelNearestNode, _PatrolWaypoints[CurrentWaypoint].GetComponent<Node_Script_OP2>());
            CurrentReturnWaypoint = 0;
            Returned = false;
        }
        else
        {
            if (CurrentReturnWaypoint >= _ReturnWaypoints.Count)
            {
                Returned = true;
            }
            //distancia
            float distance = Vector3.Distance(_ReturnWaypoints[CurrentReturnWaypoint].transform.position, _Tp2SentinelOBJ.transform.position);
            
            Vector3 Director = ((_ReturnWaypoints[CurrentReturnWaypoint].transform.position - _Tp2SentinelOBJ.transform.position).normalized * Speed);

            float DirectorAngle = MathF.Atan2(Director.y, Director.x) * Mathf.Rad2Deg;

            _Tp2SentinelOBJ.transform.position += Vector3.ClampMagnitude(Director, Speed);

            _Tp2SentinelOBJ.transform.rotation = Quaternion.Euler(Vector3.forward * DirectorAngle);

            if (distance < ArriveDist)
            {
                CurrentReturnWaypoint++;
            }
        }
    }

    public List<Transform> ReturnPatrol(List<Transform> IAPath)
    {
        IAPath.Reverse();
        _ReturnWaypoints = IAPath;
        return IAPath;
    }
}
