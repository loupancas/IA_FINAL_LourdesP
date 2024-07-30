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
}