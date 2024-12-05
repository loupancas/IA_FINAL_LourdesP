
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
            case Actions.NormalAttack:
                LeaderBase.NormalAttack();
                break;
            case Actions.Wait:
                LeaderBase.Wait();
                break;
        }
    }

    public enum Actions
    {
        
        Flee,
        NormalAttack,
        Wait,
        EspecialAttack
    }
}
