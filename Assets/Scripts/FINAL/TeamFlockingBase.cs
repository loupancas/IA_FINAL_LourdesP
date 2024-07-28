using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TeamFlockingBase : EnemigoBase
{    
   public Team Team { get; set; }

    // Bools
    public bool isFlocking;
    
    [SerializeField] ProyectilesBase _proyectil;
    [SerializeField] Transform _spawnBullet;
    [SerializeField] LayerMask _Obstacles;
    [SerializeField] LayerMask _Leader;
   
    public float _cdShot;
    List<TeamFlockingBase> _boids;

    public delegate void DelegateUpdate();
    public DelegateUpdate OnUpdate;


    protected virtual void Start()
    {
        OnUpdate = NormalUpdate;
        _vida = _vidaMax;
        AddToTeam();
        InitializeFSM();    


    }


    protected abstract void AddToTeam();

    private void InitializeFSM()
    {
        _boids = (this is TeamFlockingPinks) ? GameManager.instance.pinkAgents : GameManager.instance.cyanAgents;
        _fsm = new FSM();
        _fsm.CreateState("Attack", new EnemyAttack(_fsm, _proyectil, _spawnBullet, _Obstacles, _viewRadius, _viewAngle, _cdShot, this));
        _fsm.CreateState("Lost view", new EnemyLostView(_fsm, transform, _Obstacles, _viewRadius, _viewAngle));
        _fsm.CreateState("Movement", new EnemyMovement(_fsm, _maxVelocity, _maxForce, _viewRadius, _viewAngle, _Leader, this, _boids));
        _fsm.ChangeState("Movement");
        Debug.Log("FSM Initialized");
    }

    private void Update()
    {
        Debug.Log("Update called");
        OnUpdate?.Invoke();
    }

    public void NormalUpdate()
    {
        _fsm.Execute();
    }  
 
    public override void Morir()
    {
        //GameManager.instance.arenaManager.enemigosEnLaArena.Remove(this);
        //EnemigoVoladorFactory.Instance.ReturnProjectile(this);      
        //_vida = _vidaMax;
        gameObject.SetActive(false);
    }

}

