using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWait : IState
{
   
    

    public EnemyWait()
    {
      

    }

    public void OnEnter() 
    {
        Debug.Log("wait");
    }

    public void OnExit() { }

    public void OnUpdate()
    {
        Console.WriteLine("wait");
               
        
    }
     

  


}