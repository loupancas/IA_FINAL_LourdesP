using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    //public Agent player;
    public bool playerSpotted;
    public DecisionNode decisionTree;

    [SerializeField] LayerMask _obstacle;

    [Range(1, 15)] public float viewRadius;
    [SerializeField, Range(1, 360)] float _viewAngle;

    void Start()
    {
        //ChangeColor(myInitialMaterialColor);
    }

    // Update is called once per frame
    void Update()
    {

        //if(InFieldOfView(player.transform.position))
        //{
        //    playerSpotted = true;
        //    player.ChangeColor(Color.red);
        //}
        //else
        //{
        //    playerSpotted = false;
        //    player.ChangeColor(player.myInitialMaterialColor);
        //}

        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    decisionTree.Execute(this);
        //}
    }

    //public void SleepTime()
    //{
    //    Debug.Log($"{gameObject.name} esta en modo ZZZ");
    //}

    //public void ChaseTime()
    //{
    //    Debug.Log($"{gameObject.name} esta persiguiendo a {player.gameObject.name}");
    //}

    //public void AttackTime()
    //{
    //    Debug.Log($"{gameObject.name} esta atacando a {player.gameObject.name}");
    //}

    #region FOV
    //FOV (Field of View)
    bool InFieldOfView(Vector3 endPos)
    {
        Vector3 dir = endPos - transform.position;
        if (dir.magnitude > viewRadius) return false;
        if (!InLineOfSight(transform.position, endPos)) return false;
        if (Vector3.Angle(transform.forward, dir) > _viewAngle / 2) return false;
        return true;
    }

    //LOS (Line of Sight)
    bool InLineOfSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        return !Physics.Raycast(start, dir, dir.magnitude, _obstacle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 DirA = GetAngleFromDir(_viewAngle / 2 + transform.eulerAngles.y);
        Vector3 DirB = GetAngleFromDir(-_viewAngle / 2 + transform.eulerAngles.y);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + DirA.normalized * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + DirB.normalized * viewRadius);
    }

    Vector3 GetAngleFromDir(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion
}
