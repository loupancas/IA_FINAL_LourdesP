
using UnityEngine;

public class LeaderQuestionNode : LeaderDecisionNode
{
    public LeaderDecisionNode falseNode; //Lo que recibimos es pregunta o accion?
    public LeaderDecisionNode trueNode;

    public Questions question;

    public override void Execute(LeaderBase LeaderBase)
    {
        switch (question)
        {

            case Questions.Life:
                if (LeaderBase.Vida <= LeaderBase.healthThreshold)
                {
                    Debug.Log("Executing trueNode for Life question");
                    trueNode.Execute(LeaderBase);
                }
                else
                {
                    Debug.Log("Executing falseNode for Life question");
                    falseNode.Execute(LeaderBase);
                }
                break;
            case Questions.IsEnemyLeaderNear:
                if (LeaderBase.EnemyLeader == true)
                {
                    Debug.Log("Executing trueNode for IsLeaderNear question");
                    trueNode.Execute(LeaderBase);
                }
                else
                {
                    Debug.Log("Executing falseNode for IsLeaderNear question");
                    falseNode.Execute(LeaderBase);
                }
                break;
            case Questions.AreEnemiesVisible:
                if (LeaderBase.visibleTargets.Count > 0)
                {
                    Debug.Log("Executing trueNode for AreEnemiesVisible question");
                    trueNode.Execute(LeaderBase);
                }
                else
                {
                    Debug.Log("Executing falseNode for AreEnemiesVisible question");
                    falseNode.Execute(LeaderBase);
                }
                break;

            


        }
    }

    public enum Questions
    {
        Life,
        IsEnemyLeaderNear,
        AreEnemiesVisible,
        
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
