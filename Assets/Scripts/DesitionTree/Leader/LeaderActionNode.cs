
public class LeaderActionNode : LeaderDecisionNode
{
    public Actions action;

    public override void Execute(LeaderBase LeaderBase)
    {
        switch (action)
        {
       
            case Actions.Flee:
                LeaderBase.FleeTime();
                break;
            case Actions.EspecialAttack:
                LeaderBase.EspecialAttackTime();
                break;
            case Actions.Attack:
                LeaderBase.AttackTime();
                break;
        }
    }

    public enum Actions
    {
        
        Flee,
        Attack,
        EspecialAttack
    }
}
