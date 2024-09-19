
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
            case Questions.IsEnemyNear:
                if (Vector3.Distance(teamFlockingBase.transform.position, teamFlockingBase._Leader.transform.position) > 0.1f ) trueNode.Execute(teamFlockingBase);
                else falseNode.Execute(teamFlockingBase);
                break;
            case Questions.CanSeeLeader:
                if (teamFlockingBase.InFieldOfView(teamFlockingBase._Leader.transform.position)) trueNode.Execute(teamFlockingBase);
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
