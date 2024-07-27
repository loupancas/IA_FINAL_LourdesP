using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<EnemigoBase> allAgents = new List<EnemigoBase>();
    public List<TeamFlockingBase> pinkAgents = new List<TeamFlockingBase>();
    public List<TeamFlockingBase> cyanAgents = new List<TeamFlockingBase>();
    //public TeamFlockingPinks pink;
    //public TeamFlockingBlues cyan;
    public PlayerComp_OP2 _pinkLeader;
    public PlayerComp_Blue _blueLeader;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        //allAgents = FindObjectsOfType<EnemigoBase>().ToList();
    }
    void Start()
    {
        //SeparateAgentsIntoTeams();
        InitializeAllAgents();
        SeparateAgentsIntoTeams();

    }
    void InitializeAllAgents()
    {
        allAgents = FindObjectsOfType<EnemigoBase>().ToList();
    }
    void SeparateAgentsIntoTeams()
    {
        foreach (var agent in allAgents)
        {
            if (agent is TeamFlockingBase teamAgent)
            {
                if (teamAgent is TeamFlockingPinks)
                {
                    pinkAgents.Add(teamAgent as TeamFlockingPinks);
                }
                else if (teamAgent is TeamFlockingCyan)
                {
                    cyanAgents.Add(teamAgent as TeamFlockingCyan);
                }
            }
        }
    }

}

public enum Team { Pink, Cyan }
