using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFlockingBaseTree : MonoBehaviour
{
    public GameManager agents;
    public bool enemySpotted;
    public DecisionNode decisionTree;

    [SerializeField] LayerMask _obstacle;

    [Range(1, 15)] public float viewRadius;
    [SerializeField, Range(1, 360)] float _viewAngle;

    void Start()
    {
        //ChangeColor(myInitialMaterialColor);
        agents = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (InFieldOfView(agents.transform.position))
        {
            enemySpotted = true;
            //player.ChangeColor(Color.red);
          
        }
        else
        {
            enemySpotted = false;
            //player.ChangeColor(player.myInitialMaterialColor);

        }

       
       decisionTree.Execute(this);
        
    }

    public void SearchTime()
    {
      
    }
    public void FollowTime()
    {

    }

    public void FleeTime()
    {
       
    }

    public void AttackTime()
    {
       
    }

    #region FOV
    //FOV (Field of View)
    bool InFieldOfView(Vector3 endPos)
    {
        Vector3 dir = endPos - transform.position;
        if (dir.magnitude > viewRadius) return false;
        if (!InLineOfSight(transform.position, endPos)) return false;
        if (Vector3.Angle(transform.forward, dir) > _viewAngle / 2) return false;
        return true;
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

    public void Tree()
    {
        var root = new QuestionNode();
        var canSeePlayerNode = new QuestionNode();
        var attackNode = new ActionNode { action = ActionNode.Actions.Attack };
        var searchNode = new ActionNode { action = ActionNode.Actions.Search };
        var fleeNode = new ActionNode { action = ActionNode.Actions.Flee };

        // Configurar las relaciones entre nodos
        root.question = QuestionNode.Questions.CanSeeLeader;
        root.trueNode = canSeePlayerNode;
        root.falseNode = searchNode;

        canSeePlayerNode.question = QuestionNode.Questions.IsEnemyNear;
        canSeePlayerNode.trueNode = attackNode;
        canSeePlayerNode.falseNode = fleeNode;

        // Asignar el �rbol de decisiones al agente
        var agentTree = GetComponent<TeamFlockingBaseTree>();
        agentTree.decisionTree = root;
    }

   
}