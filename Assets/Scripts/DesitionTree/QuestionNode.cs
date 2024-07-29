
using UnityEngine;

public class QuestionNode : DecisionNode
{
    public DecisionNode falseNode; //Lo que recibimos es pregunta o accion?
    public DecisionNode trueNode;

    public Questions question;

    public override void Execute(TeamFlockingBaseTree teamFlockingBaseTree)
    {
        switch (question)
        {
            case Questions.CanSeeLeader:
                if (teamFlockingBaseTree.LiderSpotted) trueNode.Execute(teamFlockingBaseTree);
                else falseNode.Execute(teamFlockingBaseTree);
                break;
            case Questions.IsEnemyNear:
                if (Vector3.Distance(teamFlockingBaseTree.transform.position, teamFlockingBaseTree.Lider.transform.position) <
                    (teamFlockingBaseTree.viewRadius / 4)) trueNode.Execute(teamFlockingBaseTree);
                else falseNode.Execute(teamFlockingBaseTree);
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
