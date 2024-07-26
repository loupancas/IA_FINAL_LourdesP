using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;

public class Hoont : State
{

    [Header("StateMachineRefs")]
    [SerializeField] HunterStateMachine stateMachine;
    [SerializeField] Patrol PatrolState;
    [SerializeField] Rest RestState;

    [Header("References")]
    [SerializeField] HunterCore hunterCore;
    [SerializeField] Transform HunterTransform;
    [SerializeField] GameObject target;

    [Header("Variables")]
    [SerializeField] float obstacleDist;
    [SerializeField] LayerMask Obstacle;
    [SerializeField] float EatDistance;

    public void Start()
    {
        bnnuydistance = float.MaxValue;
        HunterTransform = hunterCore.transform;
    }

    private void LateUpdate()
    {
        if(target!= null) // catch target
        {
            //comenzar coroutina de check de valores
            StartCoroutine(Check()) ;
        }
    }

    public override State RunCurrentState()
    {

        if(stateMachine.HoontTime == false) // finalizo la cazeria, volver a patrullar
        {
            stateMachine.SwitchtoNewState(PatrolState);
            return PatrolState;
        }
   
        if(target == null || hunterCore.Boids.Contains(target) == false)
        {
            // catch de la existencia del conejo
            bnnuydistance= float.MaxValue;
            ChooseTarget(); // si esto corre, elegir nuevo conejo
        }

        // obstacle avoidance
        if (Physics2D.Raycast(HunterTransform.position + HunterTransform.up * 0.6f, HunterTransform.right, obstacleDist, Obstacle))
        {
            ObstacleAvoid(-1);
            return this;
        }
        else if (Physics2D.Raycast(HunterTransform.position + -HunterTransform.up * 0.6f, HunterTransform.right, obstacleDist, Obstacle))
        {

            ObstacleAvoid(1);
            return this;
        
        }
        // movimiento normal de cazador :)
        else
        {
            MovementLogic();

        }

        // el cazador alcanzo al conejo, matarlo a sangre fria en frente de su familia :D
        if(Vector3.Distance(target.transform.position, HunterTransform.position) < EatDistance)
        {
            //Destroy(target.gameObject);
            target.gameObject.SetActive(false);
            hunterCore.energy--; // restar energia
        }

        hunterCore.GetComponent<Renderer>().material.color = Color.red; // color
        return this;
    }

    Vector3 Director;
    private void MovementLogic()
    {
        //calcular distancia entre waypoint y hunter
        distance = Vector2.Distance(target.transform.position, hunterCore.transform.position);

        if(distance < 1) // ver que tipo de movimiento conviene :)
        {
            // calcular vector director SIN PROJECCION DE MOVIMIENTO
            Director = (target.transform.position - hunterCore.transform.position) * hunterCore.speed;

        }
        else
        { // calcular vector director con projeccion de movimiento
            Director = (ProjectedMovement(1) - hunterCore.transform.position) * hunterCore.speed;
        }
        // calcular angulo de rotacion
        float DirectorAngle = MathF.Atan2(Director.y, Director.x) * Mathf.Rad2Deg;

        hunterCore.transform.position += Vector3.ClampMagnitude(Director, hunterCore.speed) * Time.deltaTime;

        hunterCore.transform.rotation = Quaternion.Euler(Vector3.forward * DirectorAngle);


    }



    Vector3 V3PreviousVel, V3PreviousAccel, TargetPrevious;
    [SerializeField] Vector3 V3AvgAccl, V3AvgVel;
    IEnumerator Check()
    {
        // calculo de velocidad para projectar
        yield return new WaitForEndOfFrame();

        Vector3 V3Velocity = (target.transform.position - TargetPrevious) / Time.deltaTime;

        Vector3 V3Accel = V3Velocity - V3PreviousVel;

        V3AvgVel = V3Velocity;
        V3AvgAccl = V3Accel;

        ProjectedMovement(1); // evento de proyeccion de movimiento 

        V3PreviousVel = V3Velocity;
        V3PreviousAccel = V3Accel;
        TargetPrevious = target.transform.position;


    }

    private Vector3 ProjectedMovement(float Ftime)
    {
        Vector3 V3Ret = new Vector3();
         
        //  V3ret = VI * T + (1/2 * a * T2)  
        V3Ret = target.transform.position + (V3AvgVel * Time.deltaTime * (Ftime / Time.deltaTime)) + (0.5f * V3AvgAccl * Time.deltaTime * Mathf.Pow(Ftime / Time.deltaTime, 2));

        return V3Ret;
    }

    private void OnDrawGizmos()
    {
        if(target == null) return;  // catch :)
        Debug.DrawLine(HunterTransform.position,ProjectedMovement(1),Color.yellow);
    }

    float distance;
    float bnnuydistance;
    GameObject bnnuy;

    private void ChooseTarget()
    {
        //revisar lista de bunnies
        foreach(GameObject bunny in hunterCore.Boids)
        {
            // calculo distancia entre cazador y bunny
            distance = Vector3.Distance(hunterCore.transform.position, bunny.transform.position);

            if(distance < bnnuydistance) // si la distancia es menor a la anterior. elegir bunny
            {
                bnnuy = bunny;
                bnnuydistance = distance;
            }
        }

        hunterCore.energy -= 0.1f; // restar energia X bunny.
        target = bnnuy;
        return;
    }

     private void ObstacleAvoid(float Detector)
     {

            if(target != null) // check para evitar inconvenientes de nulls.
            {
                // doble raycast para evitar que quede atascado el hunter en un obstacle (es dificil de explicar, pero si el conejo queda atras de un obstaculo, el hunter se vuelve loco y trata de esquivar siempre)
                RaycastHit2D BnnuyHit = Physics2D.Raycast(HunterTransform.position + hunterCore.transform.up * 0.6f, (target.transform.position - hunterCore.transform.position), 20f);
                RaycastHit2D BnnuyHit2 = Physics2D.Raycast(HunterTransform.position + (-hunterCore.transform.up * 0.6f), (target.transform.position - hunterCore.transform.position), 20f);

                if(BnnuyHit.collider == null || BnnuyHit2.collider == null)
                {
                    return;
                }

                if (BnnuyHit.collider.transform == target.transform && BnnuyHit2.collider.transform == target.transform)
                {
                      // break, el hunter tiene vision directa del conejo.
                    print(BnnuyHit.collider.gameObject.tag);
                    MovementLogic();
                    return;
                }
            }
      
            // movimiento normal del hunter esquivando objetos
            Vector3 Director = new Vector3(HunterTransform.position.x, HunterTransform.transform.position.y + Detector, HunterTransform.position.z) * hunterCore.speed;
            HunterTransform.position += Vector3.ClampMagnitude(Director, hunterCore.speed) * Time.deltaTime;
            return;
     }

 
}
