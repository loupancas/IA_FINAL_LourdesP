using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] int _offsetX;
    [SerializeField] int _offsetY;
    //[SerializeField] GameObject _foodPrefab;
    [SerializeField] GameObject _obstaclePrefab;
    [SerializeField] Transform _parentContainer;
    [SerializeField] float checkRadius = 0.5f; // Radio de verificación para comprobar superposiciones
    //public int maxFood = 5;
    //public int minFood = 3;

    public int maxObstacle = 5;
    public int minObstacle = 3;

    void Awake()
    {
        List<GameObject> allSpawnedObjects = new List<GameObject>();

        //int instantiateCountFood = Random.Range(minFood, maxFood);
        int instantiateCountObstacles = Random.Range(minObstacle, maxObstacle);

        //for (int i = 0; i < instantiateCountFood; i++)
        //{
        //    GameObject food = Spawner(_foodPrefab);
        //    if (food != null)
        //        allSpawnedObjects.Add(food);
        //}

        for (int i = 0; i < instantiateCountObstacles; i++)
        {
            GameObject obstacle = Spawner(_obstaclePrefab);
            if (obstacle != null)
                allSpawnedObjects.Add(obstacle);
        }
    }

    GameObject Spawner(GameObject prefab)
    {
        Vector2 spawnPosition = RandomCoordinates();
        while (PositionIsOccupied(spawnPosition, prefab))
        {
            spawnPosition = RandomCoordinates();
        }
        return Instantiate(prefab, spawnPosition, Quaternion.identity, _parentContainer);
    }

    Vector2 RandomCoordinates()
    {
        int randomX = Random.Range(0 + _offsetX, Screen.width - _offsetX);
        int randomY = Random.Range(0 + _offsetY, Screen.height - _offsetY);

        Vector2 coordinates = new Vector2(randomX, randomY);
        return _camera.ScreenToWorldPoint(coordinates);
    }

    bool PositionIsOccupied(Vector2 position, GameObject prefabToSpawn)
    {
        float radius = prefabToSpawn.GetComponent<Collider2D>().bounds.size.magnitude / 2;
        Collider2D collider = Physics2D.OverlapCircle(position, radius + checkRadius);
        return collider != null;
    }
}
