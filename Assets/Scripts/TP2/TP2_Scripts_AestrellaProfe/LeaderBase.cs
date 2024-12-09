using System.Collections.Generic;
using UnityEngine;

public class LeaderBase : EnemigoBase
{
    public TP2_Manager_ProfeAestrella _Manager;
    public bool isMoving;
    private Queue<Vector3> pathQueue;
    public Team Team { get; set; }
    public bool EnemyLeader=false;
    public LeaderDecisionNode decisionTree;
    public FSM _fsmm;
    public LeaderProjectile _leaderProjectile;
    public Transform _spawnBullet;
    public float _cdShot;
    public Transform _home;
    public LayerMask _obstacle;
    public LayerMask _wall;
    [SerializeField] LayerMask _enemy;
    public Node_Script_OP2 NearestHomwNode;
    private bool isActionExecuting = false;
    private bool notFleeing = true;
    public float healthThreshold;
    public List<Transform> visibleTargets = new List<Transform>();
    public delegate void DelegateUpdate();
    public DelegateUpdate OnUpdate;
    public string _leaderTag;
    public bool isEvadeObstacles;


    public void NormalUpdate()
    {
       

        if (!isMoving)
        {
            if (notFleeing)
            {
                decisionTree?.Execute(this);
            }

            if (!isActionExecuting)
            {

                _fsmm.Execute();
            }
        }


    }


    public void InitializeFSM()
    {
        _fsmm = new FSM();
        _fsmm.CreateState("NormalAttack", new NormalAttack(_leaderProjectile, _spawnBullet, _cdShot));
        _fsmm.CreateState("Flee", new EnemyFlee(_home, transform, _maxVelocity, _wall, _Manager, NearestHomwNode, _obstacle, _viewRadius, isEvadeObstacles));
        _fsmm.CreateState("EspecialAttack", new EnemyEspecialAttack(_leaderProjectile, _spawnBullet, _cdShot));
        _fsmm.CreateState("Wait", new EnemyWait());
        _fsmm.ChangeState("EspecialAttack");
        //Debug.Log("FSM Initialized");
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
            behaviorText.text = "Fleeing";
            //Debug.Log("FleeTime");
            isActionExecuting = false;
        }
    }

    public void NormalAttack()
    {
        if (!isActionExecuting)
        {
            isActionExecuting = true;
            _fsmm.Execute();
            _fsmm.ChangeState("NormalAttack");
            behaviorText.text = "NormalAttack";
            //Debug.Log("NormalAttack");
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
            behaviorText.text = "EspecialAttack";
            //Debug.Log("EspecialAttackTime");
            isActionExecuting = false;
        }
    }
    
    public void Wait()
    {
        if (!isActionExecuting)
        {
            isActionExecuting = true;
            _fsmm.Execute();
            _fsmm.ChangeState("Wait");
            behaviorText.text = "Wait";
            //Debug.Log("EnemyWait");
            isActionExecuting = false;
        }
    }


    #endregion

    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        EnemyLeader = false;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _enemy);

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform targetTransform = targetsInViewRadius[i].transform;
            if (InFieldOfView(targetTransform.position))
            {
                visibleTargets.Add(targetTransform);
                Debug.Log("Enemy Spotted");

                if (targetTransform.CompareTag(_leaderTag="Leader")) 
                {
                    EnemyLeader = true;
                    //Debug.Log("Enemy Leader Spotted");
                }
               
            }
          
        
          else 
          {
            float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestTarget = targetTransform;
            }
          }


            if (closestTarget != null)
            {
                RotateTowardsTarget2D(closestTarget.position);
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
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _wall))
            {
                return true;
            }
        }
        return false;
    }


    private void RotateTowardsTarget2D(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - _spawnBullet.position).normalized;
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;

        float currentAngle = _spawnBullet.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, 180f * Time.deltaTime);

        _spawnBullet.rotation = Quaternion.Euler(0, 0, newAngle);

    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 DirA = DirFromAngle(_viewAngle / 2, false);
        Vector3 DirB = DirFromAngle(-_viewAngle / 2, false);
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



    public override void Morir()
    {
        gameObject.SetActive(false);
    }
}
