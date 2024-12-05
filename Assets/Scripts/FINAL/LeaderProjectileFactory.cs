using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderProjectileFactory : MonoBehaviour
{
    public static LeaderProjectileFactory Instance { get { return _instance; } }
    static LeaderProjectileFactory _instance;

    public LeaderProjectile projectilePrefab;
    public int stock = 10;

    public Pool<LeaderProjectile> pool;

    void Start()
    {
        _instance = this;
        pool = new Pool<LeaderProjectile>(ProjectileCreator, LeaderProjectile.TurnOnOff, stock);
    }

    public LeaderProjectile ProjectileCreator()
    {
        return Instantiate(projectilePrefab, transform);
    }

    public void ReturnProjectile(LeaderProjectile proy)
    {
        pool.ReturnObject(proy);
    }
}