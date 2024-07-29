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
    public PlayerComp_OP2 _pinkLeader;
    public PlayerComp_Blue _blueLeader;
    public Transform pinkBase;
    public Transform blueBase;
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        InitializeAllAgents();
    }
    void Start()
    {
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
                if (teamAgent.Team == Team.Pink)
                {
                    pinkAgents.Add(teamAgent);
                }
                else if (teamAgent.Team == Team.Cyan)
                {
                    cyanAgents.Add(teamAgent);
                }
            }
        }
    }

    
    public Transform GetLeader(Team team)
    {
        return team == Team.Pink ? _pinkLeader.transform : _blueLeader.transform;
    }

    public Team GetOppositeTeam(Team team)
    {
        return team == Team.Pink ? Team.Cyan : Team.Pink;
    }


}

public enum Team { Pink, Cyan }
