using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



public class Patrol : State
{
    [Header("References")]
    [SerializeField] HunterStateMachine StateMachine;
    [SerializeField] GameObject HunterObj;
    [SerializeField] Rest RestState;
    [SerializeField] Hoont HoontState;
    [SerializeField] Transform target;

    [Header("variables")]
    [SerializeField] private List<Transform> Waypoints = new List<Transform>();
    [SerializeField] float Speed, arrivaldistance, obstacledist;
    [SerializeField] LayerMask obstacles;

    public Transform HunterTransform;

    private float distance;

    private int currentwaypoint;



    public void Start()
    {
        StateMachine = GetComponentInParent<HunterStateMachine>();
        //HunterObj = GetComponentInParent<GameObject>();

        HunterTransform = HunterObj.transform;

        Speed = HunterObj.GetComponent<HunterCore>().speed;
    }

    public override State RunCurrentState()
    {
        /// check energia
        CheckEnergy();

        //cambiar color :3
        HunterObj.GetComponent<Renderer>().material.color = Color.yellow;

        //check si es momento de cazar, si lo es, es porque ingreso un conejo al radio de vision
        if (StateMachine.HoontTime == true)
        {
            StateMachine.SwitchtoNewState(HoontState);
            return HoontState;
        }

        // deteccion de obstaculos
        if (Physics2D.Raycast(HunterTransform.position + HunterTransform.up * 0.5f, HunterTransform.right, obstacledist, obstacles))
        {
            //print("dodge this you bastard");
            ObstacleAvoid(1);
            return this;

        }
        else if(Physics2D.Raycast(HunterTransform.position + -HunterTransform.up * 0.5f, HunterTransform.right, obstacledist, obstacles))
        {
            //print("parry this you filthy casul");
            ObstacleAvoid(-1);
            return this;
        }
        //correr movimiento normal
        else
        {
            MovementLogic();
        }

       
        return this;

    }

    private void MovementLogic()
    {
        if (distance <= arrivaldistance) // el hunter llego al punto de patrulla
        {
            ArriveWaypoint();
        }

        

        if (currentwaypoint >= Waypoints.Count) // second catch
        {
            currentwaypoint = 0;
        }

        //calcular distancia entre waypoint y hunter
        distance = Vector2.Distance(Waypoints[currentwaypoint].position, HunterTransform.position);

        // calcular vector director
        Vector3 Director = (Waypoints[currentwaypoint].transform.position - HunterTransform.position) * Speed;
        
        // calcular angulo de rotacion
         float DirectorAngle = MathF.Atan2(Director.y,Director.x) * Mathf.Rad2Deg;
 

        HunterTransform.position += Vector3.ClampMagnitude(Director, Speed) * Time.deltaTime;

        //HunterTransform.position = Vector2.MoveTowards(HunterTransform.position, Waypoints[currentwaypoint].transform.position, Speed * Time.deltaTime);

        //HunterTransform.right = Director;
        HunterTransform.rotation = Quaternion.Euler(Vector3.forward * DirectorAngle);
    }

    private void ObstacleAvoid(float Detector)
    {
        //print("Dodge");

        // director ahora busca moverme a la derecha / izquierda, sin variar rotacion :)
        Vector3 Director = new Vector3(HunterTransform.position.x,HunterTransform.transform.position.y + Detector,HunterTransform.position.z) * Speed;
       
        HunterTransform.position = Vector3.MoveTowards(HunterTransform.position, Director, Speed * Time.deltaTime);
    }

    private void ArriveWaypoint()
    {
        // el hunter llego al waypoint.
        // cambiar waypoint y restar energia
        
        if (currentwaypoint >= Waypoints.Count)
        {
            currentwaypoint = 0;
        }
        else
        {
            currentwaypoint++;
        }

        HunterObj.GetComponent<HunterCore>().energy--;
    }

    private void CheckEnergy()
    {
        // check energia :)
        if (StateMachine.IsCharged == false)
        {
            StateMachine.SwitchtoNewState(RestState);
            
        }

    }



    private void OnDrawGizmos()
    {
        // dibujar detectores de obstaculos
        Debug.DrawRay(HunterTransform.position + HunterTransform.up * 0.5f, HunterTransform.right, Color.green);
        Debug.DrawRay(HunterTransform.position + -HunterTransform.up * 0.5f, HunterTransform.right, Color.green);
    }

    
}
