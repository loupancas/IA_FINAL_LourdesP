using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TeamFlockingBase : EnemigoBase
{
    public Team Team { get; set; }

    // Bools
    public bool isFlocking;
    public Transform _home;
    [SerializeField] ProyectilesBase _proyectil;
    [SerializeField] Transform _spawnBullet;
    [SerializeField] LayerMask _Obstacles;
    [SerializeField] Transform _Leader;
    private Queue<Vector3> pathQueue;
    public float _cdShot;
    public TP2_Manager_ProfeAestrella pathfindingManager;
  
    public delegate void DelegateUpdate();
    public DelegateUpdate OnUpdate;
    public Node_Script_OP2 NearestNode;
    private TeamFlockingBaseTree _decisionTree;

    protected virtual void Start()
    {
        OnUpdate = NormalUpdate;
        _vida = _vidaMax;

        StartCoroutine(CorutineFindNearestNode());
        pathQueue = new Queue<Vector3>();
        InitializeFSM();
        _decisionTree = GetComponent<TeamFlockingBaseTree>();

        if (_decisionTree == null)
        {
            Debug.LogError("DecisionTree component not found.");
            return;
        }
        TP2_Manager_ProfeAestrella pathfindingManager = FindObjectOfType<TP2_Manager_ProfeAestrella>();

    }

    private void InitializeFSM()
    {
        _fsm = new FSM();
        _fsm.CreateState("Attack", new EnemyAttack(_proyectil, _spawnBullet, _cdShot));
        _fsm.CreateState("Flee", new EnemyFlee(transform, _home, _maxVelocity, _Obstacles, pathfindingManager));
        _fsm.CreateState("Movement", new EnemyMovement(_Leader,transform, _maxVelocity, _Obstacles, pathfindingManager, NearestNode));
        _fsm.ChangeState("Movement");
        Debug.Log("FSM Initialized");
    }

    protected virtual void Update()
    {
        Debug.Log("Update called");
        OnUpdate.Invoke();
        //_decisionTree.Update();
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

    IEnumerator CorutineFindNearestNode()
    {
        float Delay = 0.25f;
        WaitForSeconds wait = new WaitForSeconds(Delay);

        while (true)
        {
            yield return wait;
            NearestNode = FindNearestNode();
        }


    }
    private Node_Script_OP2 FindNearestNode()
    {
        Node_Script_OP2 nearest = null;
        float NearestVal = float.MaxValue;
        foreach (Node_Script_OP2 CurrentNode in pathfindingManager._NodeList)
        {
            float CurrentDis = Vector3.Distance(CurrentNode.NodeTransform.position, transform.position);
            if (CurrentDis < NearestVal)
            {
                NearestVal = CurrentDis;
                nearest = CurrentNode;
            }
        }
        //Debug.Log("Nearest Node: " + nearest.name);
        return nearest;

    }
}