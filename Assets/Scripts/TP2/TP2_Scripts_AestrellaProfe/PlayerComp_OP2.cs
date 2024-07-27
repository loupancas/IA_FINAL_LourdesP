using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerComp_OP2 : MonoBehaviour
{
    [SerializeField] TP2_Manager_ProfeAestrella _Manager;
    public Node_Script_OP2 NearestNode;
    public float speed;
    private Vector3 _targetPos;
    private bool isMoving;
    private Queue<Vector3> pathQueue;

    public void Start()
    {
        _Manager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        _Manager._Player = this.gameObject;
        pathQueue = new Queue<Vector3>();
        StartCoroutine(CoorutineFindNearestNode());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;
                Debug.Log("Hit Point: " + hitPoint);
                //_targetPos = hitPoint;
                //isMoving = true;

                //if (Vector3.Distance(NearestNode.transform.position, hitPoint) < 1f)
                //{
                //    _targetPos = hitPoint;
                //    isMoving = true;
                //    Debug.Log("Setting target position: " + _targetPos);
                //}

                if (Vector3.Distance(NearestNode.transform.position, hitPoint) >0.1f)
                {
                    _Manager.EndNode = _Manager.FindNodeNearPoint(hitPoint); // Encontrar el nodo más cercano al punto de impacto
                    _Manager.PathFinding(_Manager._Path, NearestNode, _Manager.EndNode);
                    pathQueue = new Queue<Vector3>(_Manager._Path.Select(node => node.position)); // Convertir el camino en una cola de posiciones
                    isMoving = true;
                    Debug.Log("Setting target path with " + pathQueue.Count + " nodes");
                }
                else
                {
                    Debug.Log("Hit point is too far from NearestNode");
                }

            }

           
        }
        //if (isMoving && Vector3.Distance(transform.position, _targetPos) > 0.1f)
        //{
        //    _Manager.PathFinding(_Manager._Path, _Manager._NearestPlayerNode, _Manager.EndNode);
        //    Vector3 moveDirection = (_targetPos - transform.position).normalized;
        //    Debug.Log("Moving towards: " + _targetPos + " with direction: " + moveDirection);
        //    transform.position += moveDirection * speed * Time.deltaTime;
        //}
        //else
        //{
        //    isMoving = false;
        //}

        if (isMoving && pathQueue.Count > 0)
        {
            Vector3 targetPos = pathQueue.Peek(); // Obtener el siguiente objetivo en el camino
            if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                Vector3 moveDirection = (targetPos - transform.position).normalized;
                Debug.Log("Moving towards: " + targetPos + " with direction: " + moveDirection);
                transform.position += moveDirection * speed * Time.deltaTime;
            }
            else
            {
                // Llegó al nodo objetivo, así que pasa al siguiente nodo
                pathQueue.Dequeue();
                if (pathQueue.Count == 0)
                {
                    isMoving = false; // Ha alcanzado el destino final
                }
            }
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
        Node_Script_OP2 nearest = null;
        foreach (Node_Script_OP2 CurrentNode in _Manager._NodeList)
        {
            float CurrentDis = Vector3.Distance(CurrentNode.NodeTransform.position, transform.position);
            if(CurrentDis < NearestVal)
            {
                NearestVal = CurrentDis;
                nearest = CurrentNode;
            }
        }
        Debug.Log("Nearest Node: " + nearest.name);
        return nearest;

    }

}
