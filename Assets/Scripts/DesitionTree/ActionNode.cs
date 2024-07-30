using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : DecisionNode
{
    public Actions action;

    public override void Execute(TeamFlockingBase teamFlockingBase)
    {
        switch (action)
        {
            case Actions.Search:
                teamFlockingBase.SearchTime();
                break;
            case Actions.Flee:
                teamFlockingBase.FleeTime();
                break;
            case Actions.Follow:
                teamFlockingBase.FollowTime();
                break;
            case Actions.Attack:
                teamFlockingBase.AttackTime();
                break;
        }
    }

    public enum Actions
    {
        Search,
        Follow,
        Flee,
        Attack
    }
}
