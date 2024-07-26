using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerComp_OP2 : MonoBehaviour
{
    [SerializeField] TP2_Manager_ProfeAestrella _Manager;
    public Node_Script_OP2 NearestNode;
    public float speed;
    private Vector3 _targetPos;
    private bool isMoving;

    public void Start()
    {
        _Manager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        _Manager._Player = this.gameObject;

        StartCoroutine(CoorutineFindNearestNode());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;

                if (Vector3.Distance(NearestNode.transform.position, hitPoint) < 1f)
                {
                    _targetPos = hitPoint;
                    isMoving = true;
                }
            }

           
        }
        if (isMoving && Vector3.Distance(transform.position, _targetPos) > 0.1f)
        {
            _Manager.PathFinding(_Manager._Path, _Manager._NearestPlayerNode, _Manager.EndNode);
            Vector3 moveDirection = (_targetPos - transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
        }
        else
        {
            isMoving = false;
        }

        _Manager._NearestPlayerNode = NearestNode;

    }

    float NearestVal = float.MaxValue;
    IEnumerator CoorutineFindNearestNode()
    {
        float Delay = 0.25f;
        WaitForSeconds wait = new WaitForSeconds(Delay);

        while (true)
        {
            yield return wait;
            NearestVal = float.MaxValue;
            NearestNode = FindNearestNode();
        }


    }
    Node_Script_OP2 nearest;
    private Node_Script_OP2 FindNearestNode()
    {
     
        foreach (Node_Script_OP2 CurrentNode in _Manager._NodeList)
        {
            float CurrentDis = Vector3.Distance(CurrentNode.NodeTransform.position, transform.position);
            if(CurrentDis < NearestVal)
            {
                NearestVal = CurrentDis;
                nearest = CurrentNode;
            }
        }
        return nearest;

    }

}
