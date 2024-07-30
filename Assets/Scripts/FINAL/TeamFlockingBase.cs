using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TeamFlockingBase : EnemigoBase
{
    public Team Team { get; set; }

    // Variables del árbol de decisiones
    
    public bool LiderSpotted;
    public DecisionNode decisionTree;
    public float healthThreshold = 0.15f;
    [SerializeField] LayerMask _obstacle;
   

    // Variables del FSM y movimiento
    public bool isFlocking;
    public Transform _home;
    [SerializeField] ProyectilesBase _proyectil;
    [SerializeField] Transform _spawnBullet;
    public Transform _Leader;
    public float _cdShot;
    public TP2_Manager_ProfeAestrella pathfindingManager;

    private Queue<Vector3> pathQueue;
    public delegate void DelegateUpdate();
    public DelegateUpdate OnUpdate;
    public Node_Script_OP2 NearestNode;
   
    private Transform _transform;

    protected virtual void Start()
    {
        OnUpdate = NormalUpdate;
        _vida = _vidaMax;

        StartCoroutine(CorutineFindNearestNode());
        pathQueue = new Queue<Vector3>();
        _transform = transform;

        InitializeFSM();

        // Verificar y asignar decisionTree
        if (decisionTree == null)
        {
            Debug.LogError("DecisionTree not assigned.");
            return;
        }

       

        pathfindingManager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
    }

    private void InitializeFSM()
    {
        _fsm = new FSM();
        _fsm.CreateState("Attack", new EnemyAttack(_proyectil, _spawnBullet, _cdShot));
        _fsm.CreateState("Flee", new EnemyFlee(transform, _home, _maxVelocity, _obstacle, pathfindingManager));
        _fsm.CreateState("Movement", new EnemyMovement(_Leader, transform, _maxVelocity, _obstacle, pathfindingManager, NearestNode));
        _fsm.ChangeState("Movement");
        Debug.Log("FSM Initialized");
    }

    protected virtual void Update()
    {
        OnUpdate.Invoke();
    }

    public void NormalUpdate()
    {
        _fsm.Execute();
        decisionTree?.Execute(this);
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
        return nearest;
    }

    #region Decision Tree Methods
    public void SearchTime()
    {
        _fsm.ChangeState("Movement");
        Debug.Log("SearchTime");
    }

    public void FollowTime()
    {
        _fsm.ChangeState("Follow");
        Debug.Log("FollowTime");
    }

    public void FleeTime()
    {
        _fsm.ChangeState("Flee");
        Debug.Log("FleeTime");
    }

    public void AttackTime()
    {
        _fsm.ChangeState("Attack");
        Debug.Log("AttackTime");
    }
    #endregion

    #region FOV
    public bool InFieldOfView(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - _transform.position).normalized;
        float angle = Vector3.Angle(_transform.forward, directionToTarget);

        if (angle <= _viewAngle / 2)
        {
            if (!Physics.Raycast(_transform.position, directionToTarget, Vector3.Distance(_transform.position, targetPosition), _obstacle))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 DirA = GetAngleFromDir(_viewAngle / 2 + transform.eulerAngles.y);
        Vector3 DirB = GetAngleFromDir(-_viewAngle / 2 + transform.eulerAngles.y);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + DirA.normalized * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + DirB.normalized * _viewRadius);
    }

    Vector3 GetAngleFromDir(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion
}
