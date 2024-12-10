using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spowner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] carPrefabs; // The car prefab to spawn

    [SerializeField]
    private Transform[] spawnPoints; // Array of spawn points in the scene

    [SerializeField]
    private float spawnInterval = 2f; // Time interval between car spawns

    [SerializeField]
    private int maxCars = 10; // Maximum number of cars allowed in the scene

    public SimulationManager simulationManager;

    private List<GameObject> activeCars = new List<GameObject>();

    public int totalCarsCount, activeCarsCount, finishedCarsCount;

    private bool spawning = true, stopSpawning = false;
    

    void Start()
    {
        StartCoroutine(SpawnCars());
    }

    private void Update()
    {
        spawning = simulationManager.simulationRuning && !stopSpawning;
    }

    private IEnumerator SpawnCars()
    {
        if (spawning)
        {
            if (activeCars.Count < maxCars)
            {
                SpawnCar();
            }

        }
            yield return new WaitForSeconds(spawnInterval);
        StartCoroutine(SpawnCars());
    }

    private void SpawnCar()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned for CarSpawner.");
            return;
        }

        // Choose a random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        int randomCar = Random.Range(0, carPrefabs.Length);
        GameObject carPrefab = carPrefabs[randomCar];

        // Spawn the car at the chosen spawn point
        GameObject newCar = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        activeCars.Add(newCar);

        CarController carController = newCar.GetComponent<CarController>();
        if (carController != null)
        {
            carController.simulationManager = simulationManager;
        }
        totalCarsCount++;
        activeCarsCount++;
        // Add a destroy callback when the car is no longer needed
        newCar.GetComponent<CarController>().OnCarDestroyed += () => RemoveCar(newCar);
    }

    private void RemoveCar(GameObject car)
    {
        if (activeCars.Contains(car))
        {
            activeCars.Remove(car);
            activeCarsCount--;
            finishedCarsCount++;
        }
    }

    public void StopSpawning()
    {
        spawning = false;
    }

    public void StartSpawning()
    {
        spawning = true;
        StartCoroutine(SpawnCars());
    }

    public void ToggleSpawning()
    {
        stopSpawning = !stopSpawning;
    }

    public void SetSpowningSpeed(float val)
    {
        spawnInterval = val;
    }
    public void ClearAll()
    {
        for (int i = activeCars.Count - 1; i >= 0; i--)
        {
            GameObject car = activeCars[i];
            if (car != null)
            {
                Destroy(car);
            }
            activeCars.RemoveAt(i);
        }

        totalCarsCount = 0;
        activeCarsCount = 0;
        finishedCarsCount = 0;
    }

}
