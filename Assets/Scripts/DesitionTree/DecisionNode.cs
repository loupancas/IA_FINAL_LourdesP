using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecisionNode : MonoBehaviour
{
    public abstract void Execute(TeamFlockingBaseTree teamFlockingBaseTree);
}
