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

    private TeamFlockingBaseTree _decisionTree;

    protected virtual void Start()
    {
        OnUpdate = NormalUpdate;
        _vida = _vidaMax;
        InitializeFSM();
        _decisionTree = GetComponent<TeamFlockingBaseTree>();

        if (_decisionTree == null)
        {
            Debug.LogError("DecisionTree component not found.");
            return;
        }
    }

    private void InitializeFSM()
    {
        TP2_Manager_ProfeAestrella pathfindingManager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        _boids = (this is TeamFlockingPinks) ? GameManager.instance.pinkAgents : GameManager.instance.cyanAgents;
        _fsm = new FSM();
        _fsm.CreateState("Attack", new EnemyAttack(_fsm, _decisionTree));
        _fsm.CreateState("Lost view", new EnemyLostView(_fsm, _decisionTree));
        _fsm.CreateState("Movement", new EnemyMovement(_fsm, _decisionTree));
        _fsm.ChangeState("Movement");
        Debug.Log("FSM Initialized");
    }

    protected virtual void Update()
    {
        Debug.Log("Update called");
        OnUpdate.Invoke();
        _decisionTree.Update();
    }

    public void NormalUpdate()
    {
        Debug.Log("FSM Execute called");
        _fsm.Execute();
    }

    public override void Morir()
    {
        gameObject.SetActive(false);
    }
}