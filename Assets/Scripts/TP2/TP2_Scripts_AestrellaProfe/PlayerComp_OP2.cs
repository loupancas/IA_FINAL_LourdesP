using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerComp_OP2 : MonoBehaviour
{
    [SerializeField] TP2_Manager_ProfeAestrella _Manager;
    public Node_Script_OP2 NearestNode;
    public float speed;

    public void Start()
    {
        _Manager = FindObjectOfType<TP2_Manager_ProfeAestrella>();
        _Manager._Player = this.gameObject;

        StartCoroutine(CoorutineFindNearestNode());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position = this.transform.position + (new Vector3(0, 1, 0) * Time.deltaTime) * speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.transform.position = this.transform.position + (new Vector3(0, -1, 0) * Time.deltaTime) * speed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            this.transform.position = this.transform.position + (new Vector3(-1, 0, 0) * Time.deltaTime) * speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.position = this.transform.position + (new Vector3(1, 0, 0) * Time.deltaTime) * speed;
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
