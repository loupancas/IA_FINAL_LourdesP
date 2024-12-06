using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFlockingBase : EnemigoBase
{
    public Team Team { get; set; }

    
    public bool LiderSpotted;
    public DecisionNode decisionTree;
    public float healthThreshold;
    [SerializeField] LayerMask _obstacle;
    [SerializeField] LayerMask _wall;
    [SerializeField] LayerMask _enemy;
    public List<Transform> visibleTargets = new List<Transform>();
    public bool isFlocking;
    public Transform _home;
    [SerializeField] Projectile _proyectil;
    [SerializeField] Transform _spawnBullet;
    public Transform _Leader;
    Transform _targetEnemy;
    public float _cdShot;
    public TP2_Manager_ProfeAestrella pathfindingManager;
    private bool notFleeing = true;
    private Queue<Vector3> pathQueue;
    public delegate void DelegateUpdate();
    public DelegateUpdate OnUpdate;
    public Node_Script_OP2 NearestNode;
    public Node_Script_OP2 NearestHomwNode;
    protected Vector3 _velocity;
    [SerializeField] private Transform _transform;
    private FSM _fsmm;
    public bool isEvadeObstacles;
    private bool isActionExecuting = false;

    protected virtual void Start()
    {
       

        OnUpdate = NormalUpdate;
        _vida = _vidaMax;
        healthThreshold = 0.3f*_vidaMax;

        StartCoroutine(CorutineFindNearestNode());
        pathQueue = new Queue<Vector3>();
        _transform = transform;
        InitializeFSM();

        if (decisionTree == null)
        {
            return;
        }


        pathfindingManager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
    }

    

    private void InitializeFSM()
    {
        _fsmm = new FSM();
        _fsmm.CreateState("Attack", new EnemyAttack(_proyectil, _spawnBullet, _cdShot));
        _fsmm.CreateState("Flee", new EnemyFlee(_home, transform, _maxVelocity, _wall, pathfindingManager, NearestHomwNode,_obstacle, _viewRadius, isEvadeObstacles));
        _fsmm.CreateState("Follow", new EnemyMovement(_Leader, transform, _maxVelocity, _wall, pathfindingManager, NearestNode, _obstacle, _viewRadius,isEvadeObstacles)); ;
        _fsmm.CreateState("Movement", new EnemyFollow(_Leader, transform, _maxVelocity, _wall, _viewRadius, _maxForce, _obstacle, isEvadeObstacles));
        _fsmm.ChangeState("Movement");
    }

    protected virtual void Update()
    {                   

        OnUpdate.Invoke();
        pathfindingManager._NearestPlayerNode = NearestNode;
        FindVisibleTargets();
       

    }

    public void NormalUpdate()
    {

        if(notFleeing)
        {
            decisionTree?.Execute(this);
        }


        if (!isActionExecuting)
        {

            _fsmm.Execute();
        }



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
            yield return new WaitForSeconds(Delay);
        }
    }

  

  

    #region Decision Tree Methods
    public void SearchTime()
    {
        if (!isActionExecuting )
        {
            isActionExecuting = true;
            _fsmm.Execute();
            _fsmm.ChangeState("Movement");
            //Debug.Log("SearchTime");
            behaviorText.text = "Searching";
            isActionExecuting = false;
        }
    }

    public void FollowTime()
    {
        if (!isActionExecuting )
        {
            isActionExecuting = true;
            _fsmm.Execute();
            _fsmm.ChangeState("Follow");
            behaviorText.text = "Follow";
            //Debug.Log("FollowTime");
            isActionExecuting = false;
        }


    }

    public void FleeTime()
    {
        if (!isActionExecuting)
        {
            isActionExecuting = true;
            notFleeing = false;
            _fsmm.Execute();
            NearestHomwNode = pathfindingManager.FindNodeNearPoint(_home.position);
            _fsmm.ChangeState("Flee");
            behaviorText.text = "Flee";
            //Debug.Log("FleeTime");
            isActionExecuting = false;
        }
    }

    public void AttackTime()
    {
        if (!isActionExecuting)
        {
            isActionExecuting = true;
            _fsmm.Execute();
            _fsmm.ChangeState("Attack");
            behaviorText.text = "Attack";
            //Debug.Log("AttackTime");
            isActionExecuting = false;
        }
    }
    #endregion

    #region FOV
 

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(_transform.position, _viewRadius, _enemy);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform targetTransform = targetsInViewRadius[i].transform;
            if (InFieldOfView(targetTransform.position))
            {
                visibleTargets.Add(targetTransform);
            }
        }
    }

    private bool InFieldOfView(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - _transform.position).normalized;
        float angle = Vector3.Angle(_transform.up, directionToTarget);

        if (angle <= _viewAngle / 2)
        {
            float distanceToTarget = Vector3.Distance(_transform.position, targetPosition);
            if (!Physics.Raycast(_transform.position, directionToTarget, distanceToTarget, _wall))
            {
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








