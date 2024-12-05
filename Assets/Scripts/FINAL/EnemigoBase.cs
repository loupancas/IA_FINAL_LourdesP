using TMPro;
using UnityEngine;


public abstract class EnemigoBase : Entity
{
    protected FSM _fsm;

    [SerializeField] protected float _maxVelocity;
    [SerializeField] protected float _maxForce;
    [SerializeField] protected float _viewRadius;
    [Range(0, 360)]
    [SerializeField] protected float _viewAngle;
    public Team team;
    [SerializeField] protected TextMeshProUGUI behaviorText;



}


