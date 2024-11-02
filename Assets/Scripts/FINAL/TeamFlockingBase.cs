using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFlockingBase : EnemigoBase
{
    public Team Team { get; set; }

    // Variables del árbol de decisiones
    
    public bool LiderSpotted;
    public DecisionNode decisionTree;
    public float healthThreshold;
    [SerializeField] LayerMask _obstacle;
    [SerializeField] LayerMask _enemy;
    public List<Transform> visibleTargets = new List<Transform>();
    // Variables del FSM y movimiento
    public bool isFlocking;
    public Transform _home;
    [SerializeField] Projectile _proyectil;
    [SerializeField] Transform _spawnBullet;
    public Transform _Leader;
    Transform _targetEnemy;
    public float _cdShot;
    public TP2_Manager_ProfeAestrella pathfindingManager;

    private Queue<Vector3> pathQueue;
    public delegate void DelegateUpdate();
    public DelegateUpdate OnUpdate;
    public Node_Script_OP2 NearestNode;
    public Node_Script_OP2 NearestEnemyNode;
    protected Vector3 _velocity;
    [SerializeField] private Transform _transform;
    private FSM _fsmm;
    
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

        // Verificar y asignar decisionTree
        if (decisionTree == null)
        {
            Debug.LogError("DecisionTree not assigned.");
            return;
        }


        pathfindingManager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        //NearestEnemyNode = FindNearestNode();
    }

    

    private void InitializeFSM()
    {
        _fsmm = new FSM();
        _fsmm.CreateState("Attack", new EnemyAttack(_proyectil, _spawnBullet, _cdShot));
        _fsmm.CreateState("Flee", new EnemyFlee(_home, transform, _maxVelocity, _obstacle, pathfindingManager, NearestEnemyNode));
        _fsmm.CreateState("Follow", new EnemyMovement(_Leader, transform, _maxVelocity, _obstacle, pathfindingManager, NearestNode));
        _fsmm.CreateState("Movement", new EnemyFollow(_Leader, transform, _maxVelocity, _obstacle, _viewRadius, _maxForce));
        _fsmm.ChangeState("Movement");
        Debug.Log("FSM Initialized");
    }

    protected virtual void Update()
    {
        

        

        OnUpdate.Invoke();
        pathfindingManager._NearestPlayerNode = NearestNode;
        FindVisibleTargets();
       

    }

    public void NormalUpdate()
    {
        Debug.Log("NormalUpdate");
        Debug.Log("Estado actual del FSM: " + _fsmm.ToString());

        decisionTree?.Execute(this);


        if (!isActionExecuting)
        {

            _fsmm.Execute();
        }



    }

    #region ForceArrive



    protected void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxVelocity);
    }

    protected Vector3 Arrive(Vector3 targetPos)
    {
        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist > _viewRadius) return Seek(targetPos);

        return Seek(targetPos, _maxVelocity * (dist / _viewRadius));
    }

    protected Vector3 Seek(Vector3 targetPos, float speed)
    {
        Vector3 desired = (targetPos - transform.position).normalized * speed;
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);
        return steering;
    }

    protected Vector3 Seek(Vector3 targetPos)
    {
        return Seek(targetPos, _maxVelocity);
    }

    #endregion
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
            //Debug.Log("Nearest Node: " + NearestNode);
            //NearestEnemyNode = pathfindingManager.FindNodeNearPoint(_home.position);
            yield return new WaitForSeconds(Delay);
        }
    }

  

    private Node_Script_OP2 FindNearestNode()
    {
        Node_Script_OP2 nearest = null;
        float NearestVal = float.MaxValue;
        foreach (Node_Script_OP2 CurrentNode in pathfindingManager._NodeList)
        {
            float CurrentDis = Vector3.Distance(CurrentNode.NodeTransform.position, _transform.position);
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
        if (!isActionExecuting )
        {
            isActionExecuting = true;
            _fsmm.Execute();
            _fsmm.ChangeState("Movement");
            Debug.Log("SearchTime");
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
          
            Debug.Log("FollowTime");
            isActionExecuting = false;
        }


    }

    public void FleeTime()
    {
        if (!isActionExecuting)
        {
            isActionExecuting = true;
            
            _fsmm.Execute();
            //NearestEnemyNode = pathfindingManager.FindNodeNearPoint(_home.position);
            _fsmm.ChangeState("Flee");
            Debug.Log("FleeTime");
            isActionExecuting = false;
        }
    }

    public void AttackTime()
    {
        if (!isActionExecuting && _vida > healthThreshold)
        {
            isActionExecuting = true;
            _fsmm.Execute();
            _fsmm.ChangeState("Attack");
            Debug.Log("AttackTime");
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
                Debug.Log("Enemy Spotted");
                //AttackTime();
            }
        }
    }

    private bool InFieldOfView(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - _transform.position).normalized;
        float angle = Vector3.Angle(_transform.forward, directionToTarget);

        if (angle <= _viewAngle / 2)
        {
            float distanceToTarget = Vector3.Distance(_transform.position, targetPosition);
            if (!Physics.Raycast(_transform.position, directionToTarget, distanceToTarget, _obstacle))
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



    IEnumerator FindTargetsWithDelay(float v)
    {
        while (true)
        {
            yield return new WaitForSeconds(v);
            FindVisibleTargets();
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








