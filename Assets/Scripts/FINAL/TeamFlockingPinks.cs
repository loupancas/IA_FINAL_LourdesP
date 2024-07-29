using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFlockingPinks : TeamFlockingBase
{
    protected override void Start()
    {
        Team = Team.Pink;
        base.Start();
    }
   
  
    protected override void Update()
    {
        base.Update();

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Gizmos.color = Color.green;

        Vector3 leftRayPos = transform.position + transform.up * 0.5f;
        Vector3 rightRayPos = transform.position - transform.up * 0.5f;

        Gizmos.DrawLine(leftRayPos, leftRayPos + transform.forward * _viewRadius);
        Gizmos.DrawLine(rightRayPos, rightRayPos + transform.forward * _viewRadius);
    }

    public override void Huir()
    {
        throw new NotImplementedException();
    }
}

    
