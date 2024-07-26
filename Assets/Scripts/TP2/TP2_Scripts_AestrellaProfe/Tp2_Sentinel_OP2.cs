using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tp2_Sentinel_OP2 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TP2_Manager_ProfeAestrella _Manager;
    [SerializeField] Tp2_SentinelStateMachine_OP2 _StateMachine;
    public List<Tp2_Sentinel_OP2> EnemiesToAlert = new List<Tp2_Sentinel_OP2>();
    [SerializeField] FieldOfViewVisualComp_OP2 _FoVScript;


    [Header("Variables")]
    public Node_Script_OP2 NearestNode;
    public Node_Script_OP2 _PlayerNearest;
    public bool EnemySpotted;

    public bool _Alarmed;

    [Header("Events")]
    public UnityEvent AlarmTrigger;

    public void Start()
    {
        _Manager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        _StateMachine = GetComponent<Tp2_SentinelStateMachine_OP2>();
        _FoVScript= GetComponent<FieldOfViewVisualComp_OP2>();
        StartCoroutine(CoorutineFindNearestNode());
        _Manager._SentinelList.Add(this);
    }

    public void Update()
    {
        EnemySpotted = _FoVScript.CanSeePlayer;
        _StateMachine.Enemyspotted = EnemySpotted;
        _StateMachine._SentinelNearestNode = NearestNode;

        _PlayerNearest = _Manager._NearestPlayerNode;
        _StateMachine._PlayernearestNode = _PlayerNearest;
        _StateMachine.Alarm = _Alarmed;

        if (EnemySpotted)
        {
            _Manager.RaiseAlarm(this);
        }
        _StateMachine.RunStateMachine();
    }

    float NearestVal = float.MaxValue;
    IEnumerator CoorutineFindNearestNode()
    {
        float Delay = 0.25f;
        WaitForSeconds wait = new WaitForSeconds(Delay);

        while (true)
        {
            yield return wait;
            NearestVal = float.MaxValue;
            NearestNode = FindNearestNode();
        }
    }

    Node_Script_OP2 nearest;
    private Node_Script_OP2 FindNearestNode()
    {
        foreach (Node_Script_OP2 CurrentNode in _Manager._NodeList)
        {
            float CurrentDis = Vector3.Distance(CurrentNode.NodeTransform.position, transform.position);
            if (CurrentDis < NearestVal)
            {
                NearestVal = CurrentDis;
                nearest = CurrentNode;
            }
        }
        return nearest;

    }

    public void SetAlarmStatus()
    {
        _Alarmed = true;
    }

}
