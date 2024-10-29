
using UnityEngine;

public class QuestionNode : DecisionNode
{
    public DecisionNode falseNode; //Lo que recibimos es pregunta o accion?
    public DecisionNode trueNode;

    public Questions question;

    public override void Execute(TeamFlockingBase teamFlockingBase)
    {
        switch (question)
        {
            
            
            case Questions.IsLeaderNear:
                if (Vector3.Distance(teamFlockingBase.transform.position, teamFlockingBase._Leader.transform.position) > 3f) trueNode.Execute(teamFlockingBase);
                else falseNode.Execute(teamFlockingBase);
                break;
            case Questions.Life:
                if (teamFlockingBase._actualLife <= teamFlockingBase.healthThreshold) trueNode.Execute(teamFlockingBase);
                else falseNode.Execute(teamFlockingBase);
                break;        
            case Questions.AreEnemiesVisible:
                if (teamFlockingBase.visibleTargets.Count > 0) trueNode.Execute(teamFlockingBase);
                else falseNode.Execute(teamFlockingBase);
                break;
            //case Questions.CanSeeLeader:
            //    if (Vector3.Distance(teamFlockingBase.transform.position, teamFlockingBase._Leader.transform.position) <= 0.1f) trueNode.Execute(teamFlockingBase);
            //    else falseNode.Execute(teamFlockingBase);
            //    break;

        }
    }

    public enum Questions
    {
        
        IsLeaderNear,
        Life,
        AreEnemiesVisible
    }


    private void OnDrawGizmos()
    {
        if (falseNode != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, falseNode.transform.position);
        }
        if (trueNode != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, trueNode.transform.position);
        }
    }
}
