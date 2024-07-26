using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FieldOfViewVisualComp_OP2 : MonoBehaviour
{
    [SerializeField] float _Radius;
    [SerializeField] float _Angle;
    [SerializeField] TP2_Manager_ProfeAestrella _Manager;
    [SerializeField] GameObject _Player;

    [SerializeField] LayerMask TargetMask;
    [SerializeField] LayerMask _Obstacle;

    public bool CanSeePlayer;

    private void Start()
    {
        _Manager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        StartCoroutine(FOVRoutine());

    }

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;

        WaitForSeconds wait = new WaitForSeconds(delay);
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangechecks = Physics.OverlapSphere(transform.position, _Radius,TargetMask);

        if(rangechecks.Length != 0 ) 
        {
            Transform _target = rangechecks[0].transform;
            Vector3 _DirectiontoTarget = (_target.position - transform.position).normalized;

            if(Vector3.Angle(transform.right,_DirectiontoTarget) < _Angle/2)
            {
                float _DistanceToTarget = Vector3.Distance(transform.position, _target.position);
                print("player is in angles");

                if (!Physics.Raycast(transform.position, _DirectiontoTarget, _DistanceToTarget, _Obstacle))
                {
                    CanSeePlayer = true;
                }
                else
                {
                    CanSeePlayer = false;
                }
            }
            else
            {
                CanSeePlayer = false;
            }
        }
        else if(CanSeePlayer == true)
        {
            CanSeePlayer = false;
        }

    }
}
