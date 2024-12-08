using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerComp_Leader : LeaderBase
{
    public Node_Script_OP2 NearestNode;
    private Queue<Vector3> pathQueue;
    public int buttton;   
    public string _KTag;
    public float _rotationSpeed;

    public void Start()
    {
       
        _vida = _vidaMax;
        healthThreshold = 0.3f * _vidaMax;
        _Manager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        pathQueue = new Queue<Vector3>();
        StartCoroutine(CorutineFindNearestNode());
        OnUpdate = NormalUpdate;
        InitializeFSM();
        _leaderTag = _KTag;

    }

   
    void Update()
    {
        if (Input.GetMouseButtonDown(buttton))
        {
            HandleMouseClick();
        }

        if (isMoving && pathQueue.Count > 0)
        {
            MoveAlongPath();
        }

         _Manager._NearestPlayerNode = NearestNode;
        FindVisibleTargets();

        OnUpdate.Invoke();
    }

    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPoint = hit.point;
            if (Vector3.Distance(transform.position, hitPoint) > 0.1f)
            {
                _Manager.EndNode = _Manager.FindNodeNearPoint(hitPoint);
                _Manager.StartNode = NearestNode;
                List<Transform> path = _Manager.CalculatePath(NearestNode, _Manager.EndNode, _wall);
                Debug.Log("Path: " + path.Count);
                pathQueue.Clear();
                pathQueue = new Queue<Vector3>(path.Select(node => node.position)); 
                isMoving = true;
            }
            else
            {
                Debug.Log("Muy cerca al NearestNode");
            }
        }
    }

    private void MoveAlongPath()
    {
        if (pathQueue.Count == 0)
        {
            isMoving = false;
            return;
        }

        Vector3 targetPos = pathQueue.Peek();


        if (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 moveDirection = (targetPos - transform.position).normalized;


            transform.position += moveDirection * _maxVelocity * Time.deltaTime;
        }
        else
        {
            pathQueue.Dequeue();
            if (pathQueue.Count == 0)
            {
                isMoving = false;
            }
        }
    }

    IEnumerator CorutineFindNearestNode()
    {
        float Delay = 0.25f;
        WaitForSeconds wait = new WaitForSeconds(Delay);

        while (true)
        {
            yield return wait;
            NearestNode = FindNearestNode();
        }
    }

    private Node_Script_OP2 FindNearestNode()
    {
        Node_Script_OP2 nearest = null;
        float NearestVal = float.MaxValue;
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


   
}
