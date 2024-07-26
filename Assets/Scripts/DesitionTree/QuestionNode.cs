using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class QuestionNode : DecisionNode
//{
//    public DecisionNode falseNode; //Lo que recibimos es pregunta o accion?
//    public DecisionNode trueNode;

//    public Questions question;

//    //public override void Execute(Dragon dragon)
//    //{
//    //    switch (question)
//    //    {
//    //        case Questions.CanSeePlayer:
//    //            if (dragon.playerSpotted) trueNode.Execute(dragon);
//    //            else falseNode.Execute(dragon);
//    //            break;
//    //        case Questions.IsPlayerNear:
//    //            if (Vector3.Distance(dragon.transform.position, dragon.player.transform.position) < 
//    //                (dragon.viewRadius / 4)) trueNode.Execute(dragon);
//    //            else falseNode.Execute(dragon);
//    //            break;
//    //    }
//    //}

//    public enum Questions
//    {
//        CanSeePlayer,
//        IsPlayerNear
//    }


//    private void OnDrawGizmos()
//    {
//        if(falseNode != null)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawLine(transform.position, falseNode.transform.position);
//        }
//        if (trueNode != null)
//        {
//            Gizmos.color = Color.green;
//            Gizmos.DrawLine(transform.position, trueNode.transform.position);
//        }
//    }
//}
