using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TeamFlockingBase : EnemigoBase
{
    public Team Team { get; set; }

    // Variables del árbol de decisiones
    
    public bool LiderSpotted;
    public DecisionNode decisionTree;
    public float healthThreshold = 0.15f;
    [SerializeField] LayerMask _obstacle;
    [SerializeField] LayerMask _enemy;
    public List<Transform> visibleTargets = new List<Transform>();
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
    private FSM _fsmm;

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
        _fsmm = new FSM();
        _fsmm.CreateState("Attack", new EnemyAttack(_proyectil, _spawnBullet, _cdShot));
        _fsmm.CreateState("Flee", new EnemyFlee(transform, _home, _maxVelocity, _obstacle, pathfindingManager));
        _fsmm.CreateState("Movement", new EnemyMovement(_Leader, transform, _maxVelocity, _obstacle, pathfindingManager, NearestNode));
        _fsmm.ChangeState("Movement");
        Debug.Log("FSM Initialized");
    }

    protected virtual void Update()
    {
        OnUpdate.Invoke();
        pathfindingManager._NearestPlayerNode = NearestNode;

    }

    public void NormalUpdate()
    {
        _fsmm.Execute();
        Debug.Log("NormalUpdate");
        decisionTree?.Execute(this);
    }

    public override void Morir()
    {
        gameObject.SetActive(false);
    }

    IEnumerator CorutineFindNearestNode()
    {
        float Delay = 0.25f;
        while (true)
        {
            NearestNode = pathfindingManager.FindNodeNearPoint(_Leader.position);
            Debug.Log("Nearest Node: " + NearestNode);
            yield return new WaitForSeconds(Delay);
        }
    }

  

    #region Decision Tree Methods
    public void SearchTime()
    {
        _fsmm.ChangeState("Movement");
        Debug.Log("SearchTime");
    }

    public void FollowTime()
    {
        _fsmm.ChangeState("Follow");
        Debug.Log("FollowTime");
    }

    public void FleeTime()
    {
        _fsmm.ChangeState("Flee");
        Debug.Log("FleeTime");
    }

    public void AttackTime()
    {
        _fsmm.ChangeState("Attack");
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
               FindTargetsWithDelay(0.1f);
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 DirA = DirFromAngle(_viewAngle / 2 , false);
        Vector3 DirB = DirFromAngle(-_viewAngle / 2 , false);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + DirA.normalized * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + DirB.normalized * _viewRadius);
    }



    IEnumerator FindTargetsWithDelay(float v)
    {
        while (true)
        {
            yield return new WaitForSeconds(v);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _enemy);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform targetTransform = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, targetTransform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacle))
                {
                    visibleTargets.Add(targetTransform);
                    Debug.Log("Enemy Spotted");
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }



}
#endregion








