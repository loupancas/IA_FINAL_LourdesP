using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : DecisionNode
{
    public Actions action;

    public override void Execute(TeamFlockingBaseTree teamFlockingBaseTree)
    {
        switch (action)
        {
            case Actions.Search:
                teamFlockingBaseTree.SearchTime();
                break;
            case Actions.Flee:
                teamFlockingBaseTree.FleeTime();
                break;
            case Actions.Attack:
                teamFlockingBaseTree.AttackTime();
                break;
        }
    }

    public enum Actions
    {
        Search,
        Flee,
        Attack
    }
}
