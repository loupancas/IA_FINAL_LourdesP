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
  
    public override void Huir()
    {
        throw new System.NotImplementedException();
    }
}
