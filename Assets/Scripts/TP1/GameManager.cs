using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<SteeringAgent> allAgents = new List<SteeringAgent>();


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

   
}
