using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : IState
{
    Projectile _proyectil;
    Transform _bulletSpawn;   
    float _cdShot;
    float _currCdShot;
    

    public EnemyAttack(Projectile proyectil, Transform bulletSpawn, float cdShot)
    {
        _proyectil = proyectil;
        _bulletSpawn = bulletSpawn;       
        _cdShot = cdShot;

    }

    public void OnEnter() 
    {
        //Debug.Log("EnemyAttack");
    }

    public void OnExit() { }

    public void OnUpdate()
    {
        //Console.WriteLine("EnemyAttack");
       
            if (_currCdShot <= 0)
            {
                _proyectil.SpawnProyectile(_bulletSpawn);
                _currCdShot = _cdShot;
            }
            else
            {
                _currCdShot -= Time.deltaTime;
            }
        
    }
     

  


}