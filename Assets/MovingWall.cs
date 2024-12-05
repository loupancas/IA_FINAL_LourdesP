using UnityEngine;
using System.Collections;

public class MovingWall : MonoBehaviour
{
    public Transform _wall;
    Vector3 position1;
    Vector3 position2;
    public float moveTime = 2.0f;
    public Transform pos1;
    public Transform pos2;


    private void Awake()
    {
        position1 = pos1.position;
        position2 = pos2.position;
        

    }
    void Start()
    {
        _wall = GetComponent<Transform>();
        StartCoroutine(MoveWall());
    }

    IEnumerator MoveWall()
    {
        while (true)
        {
            yield return StartCoroutine(MoveToPosition(position1));
            yield return new WaitForSeconds(moveTime);
            yield return StartCoroutine(MoveToPosition(position2));
            yield return new WaitForSeconds(moveTime);
        }
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        float elapsedTime = 0;
        Vector3 startingPos = _wall.transform.position;
        while (elapsedTime < moveTime)
        {
            _wall.transform.position = Vector3.Lerp(startingPos, target, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _wall.transform.position = target;
    }
}
