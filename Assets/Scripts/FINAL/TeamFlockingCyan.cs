using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamFlockingCyan : TeamFlockingBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void AddToTeam()
    {
        GameManager.instance.cyanAgents.Add(this);
    }

    public override void Morir()
    {
        throw new System.NotImplementedException();
    }
}
