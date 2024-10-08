using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : ProyectilesBase
{
    public delegate void DelegateUpdate();
    public DelegateUpdate delegateUpdate;
    public bool isEnemyProjectile;
    private Team team;
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
       EnemigoBase enemigo = collision.collider.GetComponent<EnemigoBase>();
        if (enemigo != null && enemigo.team != team)
        {
            enemigo.TakeDamage(_modifiedDmg);
            ProjectileFactory.Instance.ReturnProjectile(this);
        }


    }

    public override void SpawnProyectile(UnityEngine.Transform spawnPoint)
    {
        var p = ProjectileFactory.Instance.pool.GetObject();
        EnemigoBase shooter = spawnPoint.GetComponentInParent<EnemigoBase>();
        if (shooter != null)
        {
            p.team = shooter.team;
        }
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