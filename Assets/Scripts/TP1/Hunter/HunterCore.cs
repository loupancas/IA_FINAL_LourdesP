using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterCore : MonoBehaviour
{
    [Header("References")]
    public List<GameObject> Boids= new List<GameObject>(); // lista de conejos
    CircleCollider2D Collider2D;

    [Header("State Machine")]
    public HunterStateMachine StateMachine;

    [Header ("variables")]

    public float speed, energy,MaxEnegy, VisionRadius,rotspeed;
    protected float TimerEnergy, Pulse;
    public bool HasTarget;
  



    // Start is called before the first frame update
    void Start()
    {
        StateMachine = GetComponent<HunterStateMachine>();

        Collider2D = GetComponent<CircleCollider2D>();

        Collider2D.radius= VisionRadius;

        energy = MaxEnegy;
    }

    // Update is called once per frame
    void Update()
    {
       
        if(energy > 1) // check energia
        {
            StateMachine.IsCharged= true; // hay energia?. enviar info
            StateMachine.RunStateMachine(); // a la maquina de estados y correrla
        }
        else
        {
            StateMachine.IsCharged = false; // hay energia?. enviar info
            StateMachine.RunStateMachine();// a la maquina de estados y correrla
        }

        CheckforTargets(); // revisar si hay objetivos viables


    }

    private void CheckforTargets()
    {
       if(Boids.Count> 0) // hay boids???
       {
            HasTarget= true; // info a la maquina de estados
            StateMachine.HoontTime = true; 
       }
        else
        {
            HasTarget = false;
            StateMachine.HoontTime = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) // check de colisiones
    {

        if (collision.CompareTag("Flocker") || collision.tag == "Flocker")
        {
            // entro un conejo al radio
           Boids.Add(collision.gameObject);
        }
      
    }




    private void OnTriggerExit2D(Collider2D collision) // check de colisiones
    {
        if (collision.CompareTag("Flocker"))
        {
            // salio un conejo del radio
            Boids.Remove(collision.gameObject);
        }
        else
        {
            return;
        }
    }


}
