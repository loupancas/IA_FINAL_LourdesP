using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tp2_SentinelState_Pursue_OP2 : State
{
    [Header("References")]
    [SerializeField] TP2_Manager_ProfeAestrella _Manager;
    [SerializeField] Tp2_SentinelStateMachine_OP2 _Tp2StateMachine; // ref. al statemachine
    [SerializeField] GameObject _Tp2SentinelOBJ; // ref. gameobject del sentinel

    [Header("State References")]
    [SerializeField] Tp2Sentinel_StatePatrol_OP2 _SentinelPatrol;
    [SerializeField] Tp2_SentinelState_Search_OP2 _SentinelAlarm;

    Tp2_Sentinel_OP2 _enemy;
    [Header("Variables")]
    [SerializeField] float speed;
    [SerializeField] GameObject _Player;
    private void Start()
    {
        _Tp2StateMachine = GetComponentInParent<Tp2_SentinelStateMachine_OP2>();
        _Manager = _Tp2StateMachine._Manager;

    }


    public override State RunCurrentState()
    {
        if(_Tp2StateMachine.Enemyspotted== false)
        {
            _Tp2StateMachine.SwitchToNewState(_SentinelPatrol);
            return _SentinelPatrol;
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
            PursueLogic();
            
            /*for (int i = 0; i < _enemy.EnemiesToAlert.Count; i++)
            {
                _enemy.EnemiesToAlert[i].Alert = true;
                //logica para cambiar cambiar estado de los otros enemigos
            }*/
            return this;
        }
    }

    private void PursueLogic()
    {
        //calcular director
        Vector3 _Director = (_Player.transform.position - _Tp2SentinelOBJ.transform.position).normalized;
        float _DirectorAngle = Mathf.Atan2(_Director.y, _Director.x) * Mathf.Rad2Deg;
        _Tp2SentinelOBJ.transform.position += Vector3.ClampMagnitude(_Director, speed);
        _Tp2SentinelOBJ.transform.rotation = Quaternion.Euler(Vector3.forward * _DirectorAngle);
    }
}
