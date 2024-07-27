using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class ArenaBase : MonoBehaviour
{
    public EnemigoBase[] enemigos;
    //public GameObject[] spawnPoints;
    public List<Node_Script_OP2> nodos = new List<Node_Script_OP2>();
    public List<EnemigoBase> enemigosEnLaArena;


    public abstract void IniciarHorda();
}