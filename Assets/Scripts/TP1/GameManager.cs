using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<EnemigoBase> allAgents = new List<EnemigoBase>();
    public List<EnemigoBase> pinkAgents = new List<EnemigoBase>();
    public List<EnemigoBase> cyanAgents = new List<EnemigoBase>();
    //public TeamFlockingPinks pink;
    //public TeamFlockingBlues cyan;
    public PlayerComp_OP2 _pinkLeader;
    public PlayerComp_Blue _blueLeader;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        allAgents = FindObjectsOfType<EnemigoBase>().ToList();
    }
    void Start()
    {
        //SeparateAgentsIntoTeams();

    }

    void SeparateAgentsIntoTeams()
    {
        pinkAgents = allAgents.Where(agent => agent.team == Team.Pink).ToList();
        cyanAgents = allAgents.Where(agent => agent.team == Team.Cyan).ToList();

        foreach (var agent in pinkAgents)
        {

            if (agent is TeamFlockingBase)
            {
                ((TeamFlockingBase)agent).AddToTeam();
            }

        }

        foreach (var agent in cyanAgents)
        {
            if (agent is TeamFlockingBase)
            {
                ((TeamFlockingBase)agent).AddToTeam();
            }
        }
    }

}

public enum Team { Pink, Cyan }
