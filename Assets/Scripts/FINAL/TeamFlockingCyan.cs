using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFlockingCyan : TeamFlockingBase
{
    protected override  void Start()
    {
        Team = Team.Cyan;
        base.Start();
    }
   
    protected override void AddToTeam()
    {
        GameManager.instance.cyanAgents.Add(this);
    }

    
}