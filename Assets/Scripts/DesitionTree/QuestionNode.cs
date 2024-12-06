
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

            case Questions.Life:
                if (teamFlockingBase.Vida <= teamFlockingBase.healthThreshold)
                {
                    //Debug.Log("Executing trueNode for Life question");
                    trueNode.Execute(teamFlockingBase);
                }
                else
                {
                    //Debug.Log("Executing falseNode for Life question");
                    falseNode.Execute(teamFlockingBase);
                }
                break;
            case Questions.IsLeaderNear:
                if (Vector3.Distance(teamFlockingBase.transform.position, teamFlockingBase._Leader.transform.position) < 2f)
                {
                    //Debug.Log("Executing trueNode for IsLeaderNear question");
                    trueNode.Execute(teamFlockingBase);
                }
                else
                {
                    //Debug.Log("Executing falseNode for IsLeaderNear question");
                    falseNode.Execute(teamFlockingBase);
                }
                break;                 
            case Questions.AreEnemiesVisible:
                if (teamFlockingBase.visibleTargets.Count > 0)
                {
                    //Debug.Log("Executing trueNode for AreEnemiesVisible question");
                    trueNode.Execute(teamFlockingBase);
                }
                else
                {
                    //Debug.Log("Executing falseNode for AreEnemiesVisible question");
                    falseNode.Execute(teamFlockingBase);
                }
                break;
           

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
