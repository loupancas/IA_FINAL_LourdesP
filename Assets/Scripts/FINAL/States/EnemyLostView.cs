public class EnemyLostView : IState
{
    private FSM _fsm;
    private TeamFlockingBaseTree _tree;

    public EnemyLostView(FSM fsm, TeamFlockingBaseTree tree)
    {
        _fsm = fsm;
        _tree = tree;
    }

    public void OnEnter() { }

    public void OnExit() { }

    public void OnUpdate()
    {
        _tree.FleeTime();
    }
}