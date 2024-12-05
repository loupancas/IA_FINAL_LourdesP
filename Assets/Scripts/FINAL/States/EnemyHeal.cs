using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeal : IState
{
    float _life;
    float _healRate = 5f;


    public EnemyHeal(float healRate, float life)
    {
        _healRate = healRate;
        _life = life;
    }

    public void OnEnter()
    {
        Debug.Log("EnemyHeal");
        
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {

        if (_life < 100)
        {
            _life += _healRate * Time.deltaTime;
        }
        else
        {
            _life = 100;
        }

    }
}
