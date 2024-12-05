using UnityEngine;

public class Projectile : ProyectilesBase
{
    public delegate void DelegateUpdate();
    public DelegateUpdate delegateUpdate;
    public bool isEnemyProjectile;
    
    [SerializeField] public Team teams;
    
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
        if (enemigo != null)
        {
            Debug.Log("Colisión con enemigo del equipo: " + enemigo.team);
            if (teams != enemigo.team)
            {
                enemigo.TakeDamage(_modifiedDmg);
                ProjectileFactory.Instance.ReturnProjectile(this);
                Debug.Log("Damage es " + _modifiedDmg);
            }
        }
        else
        {
            Debug.Log("Colisión detectada con un objeto no enemigo.");
        }

    }

    public override void SpawnProyectile(UnityEngine.Transform spawnPoint)
    {
        var p = ProjectileFactory.Instance.pool.GetObject();
        EnemigoBase shooter = spawnPoint.GetComponentInParent<EnemigoBase>();
        if (shooter != null)
        {
            p.teams = shooter.team;
        }        
        p.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.rotation.normalized);
      
        Debug.Log("Disparo proyectil"+p.teams);
    }

    public void NormalUpdate()
    {
        var distanceToTravel = _modifiedSpeed * Time.deltaTime;
        transform.position += transform.up * distanceToTravel;

        _currentDistance += distanceToTravel;
        if (_currentDistance > _maxDistance)
        {
            ProjectileFactory.Instance.ReturnProjectile(this);
        }
    }

   
   

}