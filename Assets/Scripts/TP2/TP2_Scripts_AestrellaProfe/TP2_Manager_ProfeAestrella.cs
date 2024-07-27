using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Networking.Types;


public class TP2_Manager_ProfeAestrella : MonoBehaviour
{
    [Header("Variables")]

    public List<Node_Script_OP2> _NodeList= new List<Node_Script_OP2>();
    public List<Tp2_Sentinel_OP2> _SentinelList = new List<Tp2_Sentinel_OP2>(); 
    public Node_Script_OP2 StartNode, EndNode;
    public List<Transform> _Path = new List<Transform>();
    public GameObject _Player;
    public PlayerComp_OP2 _PlayerComp;
    public Node_Script_OP2 _NearestPlayerNode;

    private void Start()
    {
        _PlayerComp = _Player.GetComponent<PlayerComp_OP2>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && StartNode != null && EndNode != null)
        {
            PathFinding(_Path, StartNode, EndNode);
        }
    }
   
    public void RaiseAlarm(Tp2_Sentinel_OP2 Caller)
    {
        foreach (Tp2_Sentinel_OP2 Guard in _SentinelList)
        {
            if (Guard == Caller)
            {
                continue;
            }
            else
            {
                Guard._Alarmed = true;
            }
        }
    }

    public void PathFinding(List<Transform> _IaPath, Node_Script_OP2 start, Node_Script_OP2 goal)
    {
        PriorityQueue<Node_Script_OP2> frontier = new PriorityQueue<Node_Script_OP2>();
        frontier.Enqueue(start, 0);

        Dictionary<Node_Script_OP2, Node_Script_OP2> cameFrom = new Dictionary<Node_Script_OP2, Node_Script_OP2>();
        cameFrom.Add(start, null);

        Dictionary<Node_Script_OP2, float> costSoFar = new Dictionary<Node_Script_OP2, float>();
        costSoFar.Add(start, 0);

        foreach (var node in _NodeList)
        {
            costSoFar[node] = float.MaxValue;
        }

        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goal)
            {
                while (current != null)
                {
                    _IaPath.Add(current.transform);
                    current = cameFrom.ContainsKey(current) ? cameFrom[current] : null;
                }
                _IaPath.Reverse();
                SmoothPath(_IaPath);
                Debug.Log("Generated Path: " + string.Join(", ", _IaPath.Select(t => t.name))); // Depuración
                return;
            }

            foreach (var neighbor in current._Neighbors)
            {
                Node_Script_OP2 neighborNode = neighbor.GetComponent<Node_Script_OP2>();
                if (neighborNode == null) continue;

                float newCost = costSoFar[current] + Vector3.Distance(current.transform.position, neighbor.position);
                if (newCost < costSoFar[neighborNode])
                {
                    costSoFar[neighborNode] = newCost;
                    float priority = newCost + Heuristic(neighbor.position, goal.transform.position);
                    frontier.Enqueue(neighborNode, priority);
                    cameFrom[neighborNode] = current;
                }
            }
        }
    }

    private float Heuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    private void SmoothPath(List<Transform> path)
    {
        int current = 0;
        while (current + 2 < path.Count)
        {
            if (InLineOfSight(path[current].position, path[current + 2].position))
            {
                path.RemoveAt(current + 1);
            }
            else
            {
                current++;
            }
        }
    }

    private bool InLineOfSight(Vector3 start, Vector3 goal)
    {
        Vector3 dir = goal - start;
        return !Physics.Raycast(start, dir, dir.magnitude, LayerMask.GetMask("Obstacle"));
    }


    public Node_Script_OP2 FindNodeNearPoint(Vector3 point)
    {
        Node_Script_OP2 nearestNode = null;
        float nearestDist = float.MaxValue;

        foreach (var node in _NodeList)
        {
            float dist = Vector3.Distance(node.transform.position, point);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestNode = node;
            }
        }
        return nearestNode;
    }

}
