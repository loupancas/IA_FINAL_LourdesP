using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : IState
{
    ProyectilesBase _proyectil;
    Transform _bulletSpawn;   
    float _cdShot;
    float _currCdShot;
    

    public EnemyAttack(ProyectilesBase proyectil, Transform bulletSpawn, float cdShot)
    {
        _proyectil = proyectil;
        _bulletSpawn = bulletSpawn;       
        _cdShot = cdShot;
      
    }

    public void OnEnter() { }

    public void OnExit() { }

    public void OnUpdate()
    {
        Console.WriteLine("EnemyAttack");
       
            if (_currCdShot <= 0)
            {
                SpawnProyectile(_bulletSpawn);
                _currCdShot = _cdShot;
            }
            else
            {
                _currCdShot -= Time.deltaTime;
            }
        
    }
     

    void SpawnProyectile(Transform spawnPoint)
    {
        var p = ProjectileFactory.Instance.pool.GetObject();
        p.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.rotation.normalized);
        Debug.Log("Disparo proyectil");
    }


}