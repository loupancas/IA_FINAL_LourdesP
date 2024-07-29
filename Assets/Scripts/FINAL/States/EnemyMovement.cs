using System;

public class EnemyMovement : IState
{
    private FSM _fsm;
    private TeamFlockingBaseTree _tree;

    public EnemyMovement(FSM fsm, TeamFlockingBaseTree tree)
    {
        _fsm = fsm;
        _tree = tree;
    }

    public void OnEnter() { }

    public void OnExit() { }

    public void OnUpdate()
    {
        if (_tree != null)
        {
            _tree.SearchTime();
        }
        else
        {
            Console.WriteLine("TeamFlockingBaseTree is null in EnemyMovement");
        }
    }
}