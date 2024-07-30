
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
            case Questions.CanSeeLeader:
                if (Vector3.Distance(teamFlockingBase.transform.position, teamFlockingBase._Leader.transform.position) < 2) trueNode.Execute(teamFlockingBase);
                else falseNode.Execute(teamFlockingBase);
                break;
            case Questions.IsEnemyNear:
                if (teamFlockingBase.InFieldOfView(Vector3.forward)) trueNode.Execute(teamFlockingBase);
                else falseNode.Execute(teamFlockingBase);
                break;
        }
    }

    public enum Questions
    {
        CanSeeLeader,
        IsEnemyNear
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
