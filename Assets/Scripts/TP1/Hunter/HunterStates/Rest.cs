using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest : State
{
    [SerializeField] float pulse;
    [SerializeField] float Timer;
    [SerializeField] bool a, b;

    [SerializeField] HunterCore hunterCore;
    [SerializeField] HunterStateMachine HunterStateMachine;

    [SerializeField] float MinDistancetoHoont;

    [SerializeField] Patrol PatrolState;
    [SerializeField] Hoont HoontState;

    float distance, closestboiddistance;

    public void Start()
    {
        pulse = 0;
       hunterCore = GetComponentInParent<HunterCore>();
        closestboiddistance = float.MaxValue;
    }

    public override State RunCurrentState()
    {
        hunterCore.GetComponent<Renderer>().material.color = Color.green;
        if(pulse >= Timer)
        {
            hunterCore.energy = hunterCore.MaxEnegy;
            //ChooseNextState();
            HunterStateMachine.SwitchtoNewState(PatrolState);
            pulse = 0;
            return PatrolState;
        }
        else
        {
            pulse += Time.deltaTime;
            return this;

        }
    
        

    }

    
}
