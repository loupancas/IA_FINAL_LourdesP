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
    protected override void AddToTeam()
    {
        GameManager.instance.pinkAgents.Add(this);
    }    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Gizmos.color = Color.green;

        Vector3 leftRayPos = transform.position + transform.forward * 0.5f;
        Vector3 rightRayPos = transform.position - transform.forward * 0.5f;

        Gizmos.DrawLine(leftRayPos, leftRayPos + transform.forward * _viewAngle);
        Gizmos.DrawLine(rightRayPos, rightRayPos + transform.forward * _viewAngle);
    }

}

    
