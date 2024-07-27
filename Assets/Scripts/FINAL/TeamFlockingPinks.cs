using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFlockingPinks : TeamFlockingBase
{
    public override void AddToTeam()
    {
        GameManager.instance.pinkAgents.Add(this);
    }
}

    
