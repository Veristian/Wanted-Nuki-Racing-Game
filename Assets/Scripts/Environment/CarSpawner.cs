using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarSpawner : MonoBehaviour
{
    [Header("Car Spawner Settings")]
    public GameObject[] carPrefab; // Assign your car prefab in the inspector
    public LayerMask groundLayer; // Assign your ground layer in the inspector
    public float speed = 10f; // Speed of the car
    public float spawnInterval = 2f; // Time interval between spawns
    public float maxTime = 180f; // Distance to destroy the car after it has passed the end point

    private float spawnTimer = 0f; // Timer to track spawn intervals

    [Header("References")]
    public GameObject startPoint; // Assign your start point in the inspector
    public GameObject endPoint; // Assign your end point in the inspector


    private void Start()
    {
        startPoint.GetComponent<BoxCollider>().isTrigger = true; // Ensure the start point is a trigger
        endPoint.GetComponent<BoxCollider>().isTrigger = true; // Ensure the end point is a trigger
        if (carPrefab.Length == 0 || startPoint == null || endPoint == null)
        {
            Debug.LogError("Car prefab or start/end points not set up correctly.");
            return;
        }
        endPoint.AddComponent<DestroyCarNPC>(); // Add the destroy script to the end point

    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnCar();
            spawnTimer = 0f;
        }
    }

    private void SpawnCar()
    {
        // Randomly select a car prefab from the array
        GameObject selectedCarPrefab = carPrefab[Random.Range(0, carPrefab.Length)];

        // Instantiate the car at the start point's position and rotation
        GameObject car = Instantiate(selectedCarPrefab, startPoint.transform.position, startPoint.transform.rotation);

        // Set the car's speed and direction towards the end point
        CarNPC carNPC = car.AddComponent<CarNPC>();

        carNPC.speed = speed;
        car.transform.LookAt(endPoint.transform.position);
        carNPC.groundLayer = groundLayer; // Set the ground layer for raycasting
        

        DestroyOnDelay carDestroy = car.AddComponent<DestroyOnDelay>();
        carDestroy.delay = maxTime; // Set the delay for destruction
        carDestroy.startDestroy = true; // Start the destruction process
    }

}
