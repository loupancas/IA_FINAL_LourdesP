using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : ArenaBase
{
    public int horda;
    bool _arenaEmpezada;
    CountdownTimer _timer;
    [SerializeField] float _timeSpawn;
    public delegate void DelegateUpdate();
    public DelegateUpdate delegateUpdate;


    private void Start()
    {

        delegateUpdate = NormalUpdate;
        //GameManager.instance.pj.theWorld += StoppedTime;
        _timer = new CountdownTimer(_timeSpawn);
        _timer.OnTimerStop = IniciarHorda;
    }

    private void Update()
    {
        delegateUpdate.Invoke();
    }

    public override void IniciarHorda()
    {
        _arenaEmpezada = true;
        for (int i = 0; i < 5; i++)
        {
            SpawnEnemy();
        }

    }

    public void SpawnEnemy()
    {
        int NumeroRandom1 = Random.Range(0, spawnPoints.Length);
        //int NumeroRandom2 = Random.Range(0, enemigos.Length);
        //enemigos[NumeroRandom2].SpawnEnemy(spawnPoints[NumeroRandom1].transform);
    }

    public void StoppedTime()
    {
        _timer.Pause();

    }

    public void NormalUpdate()
    {
        //if (enemigosEnLaArena.Count == 0 && _arenaEmpezada)
        //{
        //    _arenaEmpezada = false;
        //    horda++;
        //    _timer.Start();
        //}
        //_timer.Tick(Time.deltaTime);
    }

    
    public void BackToNormal()
    {
        delegateUpdate = NormalUpdate;
        _timer.Start();
    }
}