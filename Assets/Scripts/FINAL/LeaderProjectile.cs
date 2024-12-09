using UnityEngine;

public class LeaderProjectile : ProyectilesBase
{
    public delegate void DelegateUpdate();
    public DelegateUpdate delegateUpdate;
    public bool isEnemyProjectile;
    [SerializeField] private Team teams;
    
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

    public static void TurnOnOff(LeaderProjectile p, bool active = true)
    {
        if (active)
        {
            p.Reset();
        }
        p.gameObject.SetActive(active);
    }

    private void OnCollisionEnter(Collision collision)
    {
        LeaderBase enemigo = collision.collider.GetComponent<LeaderBase>();
        EnemigoBase enemigoBase = collision.collider.GetComponent<EnemigoBase>();
        if (enemigo != null)
        {
            //Debug.Log("Colisión con enemigo del equipo: " + enemigo.team);
            if (teams != enemigo.team)
            {
                enemigo.TakeDamage(_modifiedDmg);
                LeaderProjectileFactory.Instance.ReturnProjectile(this);
                Debug.Log("Damage es " + _modifiedDmg);
            }
        }
        else if (enemigoBase != null)
        {
            if (teams != enemigoBase.team)
            {
                enemigoBase.TakeDamage(_modifiedDmg/2);
                LeaderProjectileFactory.Instance.ReturnProjectile(this);
                Debug.Log("Damage es " + _modifiedDmg);
            }
        }
        else
        {
            Debug.Log("Colisión detectada con un objeto no enemigo.");
        }

    }

    public override void SpawnProyectile(Transform spawnPoint)
    {
        var p = LeaderProjectileFactory.Instance.pool.GetObject();
        LeaderBase shooter = spawnPoint.GetComponentInParent<LeaderBase>();
        if (shooter != null)
        {
            p.teams = shooter.team;
        }        
        p.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.rotation.normalized);
        Debug.Log("Disparo proyectil especial"+p.teams);
    }

    public void NormalUpdate()
    {
        var distanceToTravel = _modifiedSpeed * Time.deltaTime;
        transform.position += transform.forward * distanceToTravel;

        _currentDistance += distanceToTravel;
        if (_currentDistance > _maxDistance)
        {
            LeaderProjectileFactory.Instance.ReturnProjectile(this);
        }
    }




}