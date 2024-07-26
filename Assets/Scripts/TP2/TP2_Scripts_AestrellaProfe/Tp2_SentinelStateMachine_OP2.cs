using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tp2_SentinelStateMachine_OP2 : MonoBehaviour
{
    [Header("References")]
    public TP2_Manager_ProfeAestrella _Manager;

    [Header("SentinelRef")]
    public Tp2_Sentinel_OP2 _Sentinel;

    [Header("StateMachine")]
    public State CurrentState;
    [SerializeField] Tp2Sentinel_StatePatrol_OP2 _StatePatrol;
    [SerializeField] Tp2_SentinelState_Pursue_OP2 _StatePursue;


    [Header("Variable logic")]
    public bool Enemyspotted;
    public Node_Script_OP2 _SentinelNearestNode;
    public Node_Script_OP2 _PlayernearestNode;
    public bool Alarm;


    private void Start()
    {
        _Manager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        SwitchToNewState(_StatePatrol);
    }

    public void update()
    {
        Enemyspotted = _Sentinel.EnemySpotted;
        _PlayernearestNode = _Manager._NearestPlayerNode;
    }

    public void RunStateMachine()
    {
        State NextState = CurrentState?.RunCurrentState();
        if (NextState != null)
        {
            SwitchToNewState(NextState);
        }
    }

    public void SwitchToNewState(State newState)
    {
        //funcion para cambiar el State ejecutado :)
        CurrentState = newState;
    }

}
