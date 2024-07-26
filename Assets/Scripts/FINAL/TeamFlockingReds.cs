using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFlockingReds : SteeringAgent
{    
    //[SerializeField, Range(0f, 2.5f)] float _alignmentWeight = 1;
    [SerializeField, Range(0f, 2.5f)] float _separationWeight = 1;
    //[SerializeField, Range(0f, 2.5f)] float _cohesionWeight = 1;
    [SerializeField] private float distanceTarget = 2.5f;
    [SerializeField] private Transform _target;
    [SerializeField] private float speed;
    // Bools
    public bool isFlocking;
    FSM _fsm;
    [SerializeField] ProyectilesBase _proyectil;
    [SerializeField] Transform _spawnBullet;

    Vector3 dir;

    void Start()
    {
              
        GameManager.instance.allAgents.Add(this);
        _fsm = new FSM();
        _fsm.CreateState("Attack", new EnemyAttack(_fsm, _proyectil, _spawnBullet, _wallLayer, _viewRadius, _viewAngle, _cdShot, this));
        _fsm.CreateState("Lost view", new EnemyLostView(_fsm, transform, _wallLayer, _viewRadius, _viewAngle));
        _fsm.CreateState("Movement", new EnemyMovement(_fsm, _maxVelocity, _maxForce, _viewRadius, _viewAngle, _wallLayer, this));

        _fsm.ChangeState("Movement");


    }




    void Update()
    {
        //Move();


        //if (Vector3.Distance(transform.position, _target.position)> 1f)
        //{
        //    Vector3 moveDirection = (_target.position - transform.position).normalized;
        //    transform.position += moveDirection * speed * Time.deltaTime;
        //}
        //else
        //{
        //    AddForce(Arrive(_target.position));
        //}

        //if (!HastToUseObstacleAvoidance())
        //{
        //    AddForce(Arrive(_target.position));
        //}

        foreach (var rb in GameManager.instance.allAgents)
        {
            if (rb.transform == transform)
            {
                continue;
            }
            if (Vector3.Distance(transform.position, rb.transform.position) <= _viewRadius)
            {
                isFlocking = true;
                Flocking();
            }
            else
            {
                isFlocking = false;
                AddForce(Arrive(_target.position));
            }
        }
            
        

        /// Behaviours Texts

       
        //if (isEvadeObstacles)
        //{
        //    behaviorText.text = "Evading";
        //}

        //if (isFlocking)
        //{
        //    behaviorText.text = "Flocking";
        //}

        //if (!isFlocking && !isEvadeObstacles)
        //{
        //    behaviorText.text = "Moving";
        //}
    }

    private void Flocking()
    {
        var boids = GameManager.instance.allAgents;
        //AddForce(Alignment(boids) * _alignmentWeight);// no es necesario
        AddForce(Separation(boids) * _separationWeight); //Se aplique un radio mas chico al actual
        //AddForce(Cohesion(boids) * _cohesionWeight);//no es necesario
    }
 
   
}
