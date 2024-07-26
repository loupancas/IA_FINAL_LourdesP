using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidBunny : SteeringAgent
{    
    [SerializeField, Range(0f, 2.5f)] float _alignmentWeight = 1;
    [SerializeField, Range(0f, 2.5f)] float _separationWeight = 1;
    [SerializeField, Range(0f, 2.5f)] float _cohesionWeight = 1;
    [SerializeField] Transform _hunter;
    [SerializeField] Camera _camera;
    [SerializeField] int _offsetX;
    [SerializeField] int _offsetY;
    [SerializeField] private float distanceTarget = 2.5f;
    int _randomX;
    int _randomY;
    public Vector2 currentTargetPoint;
    public LayerMask foodLayer;
    public float detectionFoodRadius = 1f;
    public float arriveFoodSpeed = 2f;
    [SerializeField] private Transform _target;
    private GameObject targetObject;
    public float hunterRadius;

    // Bools
    public bool isFlee;
    public bool detectFood;
    public bool isEating;
    public bool isFlocking;

    Vector3 dir;

    void Start()
    {
        targetObject = new GameObject("TargetObject");
        _target = targetObject.transform;

        ChangePositions();

        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);

        dir = new Vector3(x, y);
        _velocity = dir.normalized * _maxSpeed;
        GameManager.instance.allAgents.Add(this);
    }

    void ChangePositions()
    {
        Vector2 startPosition = RandomCoordinates();
        gameObject.transform.position = startPosition;
    }

    Vector2 RandomCoordinates()
    {
        _randomX = Random.Range(0 + _offsetX, Screen.width - _offsetX);
        _randomY = Random.Range(0 + _offsetY, Screen.height - _offsetY);

        Vector2 coordinates = new Vector2(_randomX, _randomY);
        Vector2 screenToWorldPosition = _camera.ScreenToWorldPoint(coordinates);
        return screenToWorldPosition;
    }

    void Update()
    {
        Move();

        if (!isEating && !detectFood && _target.name != "Food(Clone)")
        {
            _target.position = transform.position + dir * distanceTarget;
        }

        if (Vector3.Distance(transform.position, _hunter.position) <= hunterRadius && _hunter.gameObject.activeInHierarchy)
        {
            isFlee = true;
            AddForce(Evade(_hunter.position));
        }
        else
        {
            isFlee = false;

            if (!HastToUseObstacleAvoidance())
            {
                AddForce(Arrive(_target.position));
            }

            DetectFood();

            if (!detectFood || !isEating)
            {
                foreach (var rb in GameManager.instance.allAgents)
                {
                    if (rb.transform == transform)
                    {
                        continue;
                    }
                    if (Vector3.Distance(transform.position, rb.transform.position) <= _viewRadius)
                    {
                        isFlocking = true;
                        Flocking();
                    }
                    else
                    {
                        isFlocking = false;
                    }
                }
            }
        }

        /// Behaviours Texts

        if (isFlee)
        {
            behaviorText.text = "Fleeing";
        }

        if (isEvadeObstacles)
        {
            behaviorText.text = "Evading";
        }

        if (detectFood)
        {
            behaviorText.text = "Smelling";
        }

        if (isEating)
        {
            behaviorText.text = "Eating";
        }

        if (isFlocking)
        {
            behaviorText.text = "Flocking";
        }

        if (!isFlee && !detectFood && !isEating && !isFlocking && !isEvadeObstacles)
        {
            behaviorText.text = "Moving";
        }
    }

    private void Flocking()
    {
        var boids = GameManager.instance.allAgents;
        AddForce(Alignment(boids) * _alignmentWeight);
        AddForce(Separation(boids) * _separationWeight); //Se aplique un radio mas chico al actual
        AddForce(Cohesion(boids) * _cohesionWeight);
    }

    private void DetectFood()
    {
        Collider2D foodCollider = Physics2D.OverlapCircle(transform.position,detectionFoodRadius,foodLayer);
        if(foodCollider!=null && foodCollider.CompareTag("Food"))
        {
            detectFood = true;
            _target = foodCollider.transform;

            if (Vector2.Distance(transform.position, foodCollider.transform.position) < 0.05f)
            {
                detectFood = false;
                isEating = true;

                StartCoroutine(Eat(foodCollider));
            }
        }
        else
        {
            detectFood = false;
        }
    }
 
    private IEnumerator Eat(Collider2D other)
    {
        yield return new WaitForSeconds(2f);

        other.gameObject.SetActive(false);
        _target = targetObject.transform;
        isEating = false;
    }
}
