using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLimits : MonoBehaviour
{
    [SerializeField] private float _value;
    private Camera _camera;

    void Start()
    {
       _camera = Camera.main;
        

    }

    void Update()
    {
        Vector3 ViewPortPosition = _camera.WorldToViewportPoint(transform.position);

        if(ViewPortPosition.x>1)
        {
            transform.position = _camera.ViewportToWorldPoint(new Vector3(_value, ViewPortPosition.y, ViewPortPosition.z));
        }
        else if (ViewPortPosition.x < 0)
        {
            transform.position = _camera.ViewportToWorldPoint(new Vector3(1-_value, ViewPortPosition.y, ViewPortPosition.z));
        }

        if (ViewPortPosition.y > 1)
        {
            transform.position = _camera.ViewportToWorldPoint(new Vector3( ViewPortPosition.x,_value, ViewPortPosition.z));
        }
        else if (ViewPortPosition.y < 0)
        {
            transform.position = _camera.ViewportToWorldPoint(new Vector3(ViewPortPosition.x, 1 - _value, ViewPortPosition.z));
        }

    }

    


}
