using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteeringAgent : MonoBehaviour
{
    [SerializeField] protected float _maxSpeed, _maxForce;
    [SerializeField] protected float _viewRadius;
    [SerializeField] protected LayerMask _obstacles;
    [SerializeField] protected Text behaviorText;

    protected Vector3 _velocity;

    public bool isEvadeObstacles;

    protected void Move()
    {
        transform.position += _velocity * Time.deltaTime;
        if (_velocity != Vector3.zero) transform.right = _velocity;
    }

    protected bool HastToUseObstacleAvoidance()
    {
        Vector3 avoidanceObs = ObstacleAvoidance();
        AddForce(avoidanceObs);
        return avoidanceObs != Vector3.zero;
    }

    protected Vector3 Seek(Vector3 targetPos)
    {
        return Seek(targetPos, _maxSpeed);
    }

    protected Vector3 Seek(Vector3 targetPos, float speed)
    {
        Vector3 desired = (targetPos - transform.position).normalized * speed;
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);
        return steering;
    }

    protected Vector3 Flee(Vector3 targetPos)
    {
        return -Seek(targetPos);
    }

    protected Vector3 Arrive(Vector3 targetPos)
    {
        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist > _viewRadius) return Seek(targetPos);

        return Seek(targetPos, _maxSpeed * (dist / _viewRadius));
    }
    protected Vector3 ObstacleAvoidance()
    {
        if (Physics2D.Raycast(transform.position + transform.up * 0.5f, transform.right, _viewRadius, _obstacles))
        {
            isEvadeObstacles = true;
            return Seek(transform.position - transform.up);
        }
        else if (Physics2D.Raycast(transform.position - transform.up * 0.5f, transform.right, _viewRadius, _obstacles))
        {
            isEvadeObstacles = true;
            return Seek(transform.position + transform.up);
        }

        isEvadeObstacles = false;
        return Vector3.zero;
    }

    protected Vector3 Pursuit(Vector3 targetPos)
    {
        Vector3 futurePos = targetPos + _velocity;
        Debug.DrawLine(transform.position, futurePos, Color.cyan);
        return Seek(futurePos);
    }

    protected Vector3 Evade(Vector3 targetPos)
    {
        return -Pursuit(targetPos);
    }

    public void ResetPosition() //cuando eliminan un conejo
    {
        transform.position = Vector3.zero;
    }

    protected Vector3 Alignment(List<SteeringAgent> agents)
    {
        Vector3 desired = Vector3.zero;
        int boidsCount = 0;

        foreach (var item in agents)
        {
            if (Vector3.Distance(item.transform.position, transform.position) > _viewRadius) continue;
            desired += item._velocity;
            boidsCount++;
        }

        desired /= boidsCount;
        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    protected Vector3 Separation(List<SteeringAgent> agents)
    {
        Vector3 desired = Vector3.zero;

        foreach (var item in agents)
        {
            if (item == this) continue; //Ignorar mi propio calculo
            Vector3 dist = item.transform.position - transform.position;

            if (dist.sqrMagnitude > _viewRadius * _viewRadius) 
            {
                continue;
            }

            desired += dist;
        }

        if (desired == Vector3.zero) return Vector3.zero;
        desired *= -1;
        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    protected Vector3 Cohesion(List<SteeringAgent> agents)
    {
        Vector3 desired = Vector3.zero;
        int boidsCount = 0;

        foreach (var item in agents)
        {
            if (item == this) continue; //Ignorar mi propio calculo

            Vector3 dist = item.transform.position - transform.position;

            if (dist.sqrMagnitude > _viewRadius * _viewRadius) continue;

            //Promedio = Suma / Cantidad
            desired += item.transform.position;
            boidsCount++;
        }

        if (boidsCount == 0) return Vector3.zero; //Si no hay agentes

        desired /= boidsCount;

        return Seek(desired);
    }

    protected Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude(desired - _velocity, _maxForce * Time.deltaTime);
    }

    protected void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    } 
}
