using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderBase : EnemigoBase
{
    [SerializeField] TP2_Manager_ProfeAestrella _Manager;
    public Node_Script_OP2 NearestNode;
    public float speed;
    private bool isMoving;
    private Queue<Vector3> pathQueue;
    public LayerMask LayerMask;
    public Team Team { get; set; }
    public bool EnemyLeader=false;
    public LeaderDecisionNode decisionTree;
    private FSM _fsmm;
    [SerializeField] Projectile _proyectil;
    [SerializeField] Transform _spawnBullet;
    public float _cdShot;
    public Transform _home;
    [SerializeField] LayerMask _obstacle;
    [SerializeField] LayerMask _enemy;
    public Node_Script_OP2 NearestHomwNode;
    private bool isActionExecuting = false;
    private bool notFleeing = true;
    public float healthThreshold;
    public List<Transform> visibleTargets = new List<Transform>();

  

    protected virtual void start()
    {
        Team = Team.Pink;
        _vida = _vidaMax;
        healthThreshold = 0.3f * _vidaMax;
        _Manager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        pathQueue = new Queue<Vector3>();
        StartCoroutine(CorutineFindNearestNode());
        InitializeFSM();

        if (decisionTree == null)
        {
            Debug.LogError("DecisionTree not assigned.");
            return;
        }
    }


    protected virtual void Update()
    {
        NearestNode = FindNearestNode();
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        if (isMoving && pathQueue.Count > 0)
        {
            MoveAlongPath();
        }

        //_Manager._NearestPlayerNode = NearestNode;
        FindVisibleTargets();
    }

    public void NormalUpdate()
    {
        Debug.Log("NormalUpdate");

        if (notFleeing)
        {
            decisionTree?.Execute(this);
        }


        if (!isActionExecuting)
        {

            _fsmm.Execute();
        }



    }


    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPoint = hit.point;
            if (Vector3.Distance(transform.position, hitPoint) > 0.1f)
            {
                Node_Script_OP2 endNode = _Manager.FindNodeNearPoint(hitPoint);
                if (endNode != null && NearestNode != null)
                {
                    _Manager.EndNode = endNode;
                    _Manager.StartNode = NearestNode;
                    List<Transform> path = _Manager.CalculatePath(NearestNode, _Manager.EndNode, LayerMask);
                    //pathQueue.Clear();
                    pathQueue = new Queue<Vector3>(path.Select(node => node.position)); // Usar path en lugar de _Manager._Path
                    isMoving = true;
                }
                else
                {
                    Debug.LogError("Start or End node is null.");
                }
            }
            else
            {
                Debug.Log("Hit point is too close to the NearestNode");
            }
        }
    }

    private void MoveAlongPath()
    {
        if (pathQueue.Count == 0)
        {
            isMoving = false;
            return;
        }

        Vector3 targetPos = pathQueue.Peek();
        if (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 moveDirection = (targetPos - transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
        }
        else
        {
            pathQueue.Dequeue();
            if (pathQueue.Count == 0)
            {
                isMoving = false;
            }
        }
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
        foreach (Node_Script_OP2 CurrentNode in _Manager._NodeList)
        {
            float CurrentDis = Vector3.Distance(CurrentNode.NodeTransform.position, transform.position);
            if (CurrentDis < NearestVal)
            {
                NearestVal = CurrentDis;
                nearest = CurrentNode;
            }
        }
        Debug.Log("Nearest Node: " + nearest);
        return nearest;
    }


    private void InitializeFSM()
    {
        _fsmm = new FSM();
        _fsmm.CreateState("Attack", new EnemyAttack(_proyectil, _spawnBullet, _cdShot));
        _fsmm.CreateState("Flee", new EnemyFlee(_home, transform, _maxVelocity, _obstacle, _Manager, NearestHomwNode));
        _fsmm.CreateState("EspecialAttack", new EnemyEspecialAttack(_proyectil, _spawnBullet, _cdShot));
        _fsmm.ChangeState("EspecialAttack");
        Debug.Log("FSM Initialized");
    }

    #region Decision Tree Methods
  

    public void FleeTime()
    {
        if (!isActionExecuting)
        {
            isActionExecuting = true;
            notFleeing = false;
            _fsmm.Execute();
            NearestHomwNode = _Manager.FindNodeNearPoint(_home.position);
            _fsmm.ChangeState("Flee");
            Debug.Log("FleeTime");
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
            Debug.Log("AttackTime");
            isActionExecuting = false;
        }
    }


    public void EspecialAttackTime()
    {
        if (!isActionExecuting)
        {
            isActionExecuting = true;
            _fsmm.Execute();
            _fsmm.ChangeState("EspecialAttack");
            Debug.Log("EspecialAttackTime");
            isActionExecuting = false;
        }
    }


    #endregion

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _enemy);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform targetTransform = targetsInViewRadius[i].transform;
            if (InFieldOfView(targetTransform.position))
            {
                //visibleTargets.Add(targetTransform);
                //Debug.Log("Enemy Spotted");

                if (targetTransform.CompareTag("Leader")) // Verificar si el enemigo es un líder
                {
                    EnemyLeader = true;
                    Debug.Log("Enemy Leader Spotted");
                }
                else
                {
                    visibleTargets.Add(targetTransform);
                    Debug.Log("Enemy Spotted");
                }

            }
        }
    }

    private bool InFieldOfView(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angle = Vector3.Angle(transform.up, directionToTarget);

        if (angle <= _viewAngle / 2)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacle))
            {
                return true;
            }
        }
        return false;
    }

    public override void Morir()
    {
        gameObject.SetActive(false);
    }
}
