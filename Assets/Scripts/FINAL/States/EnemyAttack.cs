public class EnemyAttack : IState
{
    private FSM _fsm;
    private TeamFlockingBaseTree _tree;

    public EnemyAttack(FSM fsm, TeamFlockingBaseTree tree)
    {
        _fsm = fsm;
        _tree = tree;
    }

    public void OnEnter() { }

    public void OnExit() { }

    public void OnUpdate()
    {
        _tree.AttackTime();
    }
}