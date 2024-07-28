using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFlockingPinks : TeamFlockingBase
{
    private void Start()
    {
        
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

        Vector3 leftRayPos = transform.position + transform.up * 0.5f;
        Vector3 rightRayPos = transform.position - transform.up * 0.5f;

        Gizmos.DrawLine(leftRayPos, leftRayPos + transform.right * _viewAngle);
        Gizmos.DrawLine(rightRayPos, rightRayPos + transform.right * _viewAngle);
    }

}

    
