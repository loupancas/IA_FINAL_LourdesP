using UnityEngine;

public abstract class EnemigoBase : Entity
{
    protected FSM _fsm;

    [SerializeField] protected float _maxVelocity;
    [SerializeField] protected float _maxForce;
    [SerializeField] protected float _viewRadius;
    [SerializeField] protected float _viewAngle;
    [SerializeField] protected Transform _leader;
    public Team team;

   

}