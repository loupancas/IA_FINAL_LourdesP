using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamFlockingBaseTree : MonoBehaviour
{
    public Transform Lider;
    public bool LiderSpotted;
    public DecisionNode decisionTree;
    public float healthThreshold = 0.15f;
    [SerializeField] LayerMask _obstacle;

    [Range(1, 15)] public float viewRadius;
    [SerializeField, Range(1, 360)] float _viewAngle;

    private FSM _fsm;
    private Transform _transform;
    

    void Start()
    {

        _transform = transform;

        //InitializeFSM();



        Lider = GetComponent<Transform>();
        if (Lider == null)
        {
            Debug.LogError("GameManager component not found.");
            return;
        }

        // Verificar si decisionTree está asignado
        if (decisionTree == null)
        {
            Debug.LogError("DecisionTree not assigned.");
            return;
        };
    }

    void Update()
    {

        if (InFieldOfView(Lider.transform.position))
        {
            LiderSpotted = true;
            //player.ChangeColor(Color.red);
          
        }
        else
        {
            LiderSpotted = false;
            //player.ChangeColor(player.myInitialMaterialColor);

        }

       
       //decisionTree?.Execute(this);
        _fsm.Execute();
    }

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

    //private void InitializeFSM()
    //{
    //    _fsm = new FSM();
    //    _fsm.CreateState("Attack", new EnemyAttack());
    //    _fsm.CreateState("Flee", new EnemyFlee(transform, GameManager.instance.GetBasePosition(Team), 5f, _obstacle));
    //    _fsm.CreateState("Movement", new EnemyMovement(Lider, transform, 5f, _obstacle));
    //    _fsm.ChangeState("Movement");
    //    Debug.Log("FSM Initialized");
    //}


    #region FOV
    //FOV (Field of View)
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
    //LOS (Line of Sight)
    bool InLineOfSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        return !Physics.Raycast(start, dir, dir.magnitude, _obstacle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 DirA = GetAngleFromDir(_viewAngle / 2 + transform.eulerAngles.y);
        Vector3 DirB = GetAngleFromDir(-_viewAngle / 2 + transform.eulerAngles.y);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + DirA.normalized * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + DirB.normalized * viewRadius);
    }

    Vector3 GetAngleFromDir(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion

    private bool InFOV(Transform obj)
    {
        Vector3 dir = obj.position - _transform.position;
        return dir.magnitude < viewRadius && Vector3.Angle(_transform.forward, dir) <= _viewAngle * 0.5f && InLineOfSight(_transform.position, obj.position);
    }


}
