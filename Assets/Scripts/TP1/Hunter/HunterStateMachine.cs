using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterStateMachine : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] HunterCore HunterCore;


    [Header("State Machine")]
    public State CurrentState; // variable del state machine, permite definir que variable usar


    [SerializeField] Rest RestState; // referencia del estado
    [SerializeField] Patrol PatrolState; // referencia del estado
    [SerializeField] Hoont HoontState; // referencia del estado


    [Header("Variables & configs")]
    public bool IsCharged;
    public bool IsPatrol;
    public bool HoontTime;


    public void Start()
    {
        SwitchtoNewState(PatrolState);
    }


    void Update()
    {
        
    }


   public void RunStateMachine()
    {
        // primero pregunto si hay estado elegido.
        //si lo hay, correr el evento de ejecucion.
        if(!IsCharged)
        {
            SwitchtoNewState(RestState);
        }


        State NextState = CurrentState?.RunCurrentState(); 

        if(NextState != null )
        {
            SwitchtoNewState(NextState);
        }
    }

    public void SwitchtoNewState(State NewState)
    {
        //funcion para cambiar el State ejecutado :)
        CurrentState= NewState;
    }
}
