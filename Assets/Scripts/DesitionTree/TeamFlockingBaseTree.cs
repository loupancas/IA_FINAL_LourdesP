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
    private TeamFlockingBase _me;
    private Transform _transform;
    private TP2_Manager_ProfeAestrella _pathfindingManager;


    void Start()
    {
        _me = GetComponent<TeamFlockingBase>();

        _transform = transform;

        _pathfindingManager = FindObjectOfType<TP2_Manager_ProfeAestrella>();  


        _fsm = new FSM();

        if (_pathfindingManager == null)
        {
            Debug.LogError("TP2_Manager_ProfeAestrella component not found.");
            return;
        }

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

    public void Update()
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

       
       decisionTree?.Execute(this);
        
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
     



    #region FOV
    //FOV (Field of View)
    bool InFieldOfView(Vector3 endPos)
    {
        //Vector3 dir = endPos - transform.position;
        //if (dir.magnitude > viewRadius) return false;
        //if (!InLineOfSight(transform.position, endPos)) return false;
        //if (Vector3.Angle(transform.forward, dir) > _viewAngle / 2) return false;
        //return true;

        Vector3 dir = endPos - transform.position;
        return dir.magnitude <= viewRadius && InLineOfSight(transform.position, endPos) && Vector3.Angle(transform.forward, dir) <= _viewAngle / 2;

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
