using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Projectile : ProyectilesBase
{
    public bool devuelto = false;
    public delegate void DelegateUpdate();
    public DelegateUpdate delegateUpdate;


    void Start()
    {
        delegateUpdate = NormalUpdate;       
        _modifiedDmg = _dmg;
        _modifiedSpeed = _speed;
    }


    void Update()
    {
        delegateUpdate.Invoke();


    }

    private void Reset()
    {
        _currentDistance = 0;
        _modifiedDmg = _dmg;
        _modifiedSpeed = _speed;
        devuelto = false;
        delegateUpdate = NormalUpdate;
    }

    public static void TurnOnOff(Projectile p, bool active = true)
    {
        if (active)
        {
            p.Reset();
        }
        p.gameObject.SetActive(active);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //var targetLeader = GameManager.instance.GetLeader(_me.Team);
        if (collision.collider.GetComponent<EnemigoBase>() != null)
        {
            collision.collider.GetComponent<EnemigoBase>().TakeDamage(_modifiedDmg);
            
            ProjectileFactory.Instance.ReturnProjectile(this);
        }

        if (collision.collider.GetComponent<EnemigoBase>() != null && devuelto)
        {
            print("Toco al enemigo");
            collision.collider.GetComponent<EnemigoBase>().Morir();
           

            ProjectileFactory.Instance.ReturnProjectile(this);
        }

    }

    public override void SpawnProyectile(UnityEngine.Transform spawnPoint)
    {
        var p = ProjectileFactory.Instance.pool.GetObject();
        p.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.rotation.normalized);
        Debug.Log("Disparo proyectil");
    }

    public void NormalUpdate()
    {
        var distanceToTravel = _modifiedSpeed * Time.deltaTime;
        transform.position += transform.forward * distanceToTravel;

        _currentDistance += distanceToTravel;
        if (_currentDistance > _maxDistance)
        {
            ProjectileFactory.Instance.ReturnProjectile(this);
        }
    }

   
   

}