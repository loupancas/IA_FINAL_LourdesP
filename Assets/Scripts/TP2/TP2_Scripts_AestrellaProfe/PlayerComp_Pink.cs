using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerComp_Pink : MonoBehaviour
{
    [SerializeField] TP2_Manager_ProfeAestrella _Manager;
    public Node_Script_OP2 NearestNode;
    public float speed;
    private Vector3 _targetPos;
    private bool isMoving;
    private Queue<Vector3> pathQueue;
    public LayerMask LayerMask;
    public Team Team { get; set; }
    public void Start()
    {
        Team = Team.Pink;
        _Manager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        _Manager._Player = this.gameObject;
        pathQueue = new Queue<Vector3>();
        StartCoroutine(CoorutineFindNearestNode());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        if (isMoving && pathQueue.Count > 0)
        {
            MoveAlongPath();
        }

        _Manager._NearestPlayerNode = NearestNode;

       
    }

    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 hitPoint = hit.point;
            //Debug.Log("Hit Point: " + hitPoint);

            if (Vector3.Distance(transform.position, hitPoint) > 0.1f)
            {
                _Manager.EndNode = _Manager.FindNodeNearPoint(hitPoint);
                _Manager.StartNode = NearestNode;
                _Manager.PathFinding(_Manager._Path, NearestNode, _Manager.EndNode,LayerMask);
                pathQueue.Clear();
                pathQueue = new Queue<Vector3>(_Manager._Path.Select(node => node.position));
                isMoving = true;
              
            }
            else
            {
                Debug.Log("Hit point is too close to the NearestNode");
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
            //Debug.Log("Moving towards: " + targetPos + " with direction: " + moveDirection);
            transform.position += moveDirection * speed * Time.deltaTime;
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


    float NearestVal = float.MaxValue;
    IEnumerator CoorutineFindNearestNode()
    {
        float Delay = 0.25f;
        WaitForSeconds wait = new WaitForSeconds(Delay);

        while (true)
        {
            yield return wait;
            NearestNode = FindNearestNode();
        }


    }
    Node_Script_OP2 nearest;
    private Node_Script_OP2 FindNearestNode()
    {
        Node_Script_OP2 nearest = null;
        float NearestVal = float.MaxValue;
        foreach (Node_Script_OP2 CurrentNode in _Manager._NodeList)
        {
            float CurrentDis = Vector3.Distance(CurrentNode.NodeTransform.position, transform.position);
            if(CurrentDis < NearestVal)
            {
                NearestVal = CurrentDis;
                nearest = CurrentNode;
            }
        }
        //Debug.Log("Nearest Node: " + nearest.name);
        return nearest;

    }

}
