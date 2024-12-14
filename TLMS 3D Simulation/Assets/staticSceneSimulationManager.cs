using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class staticSceneSimulationManager : MonoBehaviour
{
    public TextMeshProUGUI simulationTimeText;
    public GameObject startSimulationButton, pauseSimulationButton, playSimulationButton, stopSimulationButton, camerasPanel;

    private float elapsedTime;

    public bool simulationsRuning { get; private set; }

    public int roadNumPerSim;


    public SimulationManager[] simulationObject;

    public UnityEvent<int> onAddCar;

    public float soawnInterval;

    private bool cameraToggeler = false;

    //public List<GameObject> Simulations = new List<GameObject>();
    private void Start()
    {
        camerasPanel.SetActive(false);
        cameraToggeler = false;
        startSimulationButton.SetActive(true);
        pauseSimulationButton.SetActive(false);
        playSimulationButton.SetActive(false);
        stopSimulationButton.SetActive(false);
        StartCoroutine(addACar());
    }


    public void toggelCamerasPanel()
    {
        cameraToggeler = !cameraToggeler;
        camerasPanel.SetActive(cameraToggeler);
    }

    public void StartSimulations()
    {
        elapsedTime = 0f;
        simulationsRuning = true;
        startSimulationButton.SetActive(false);
        pauseSimulationButton.SetActive(true);
        playSimulationButton.SetActive(false);
        stopSimulationButton.SetActive(true);

        foreach (var sim in simulationObject)
        {
                sim.startSimulation();
        }
    }

    public void PauseSimulation()
    {
        simulationsRuning = false;
        startSimulationButton.SetActive(false);
        pauseSimulationButton.SetActive(false);
        playSimulationButton.SetActive(true);
        stopSimulationButton.SetActive(true);

        foreach (var sim in simulationObject)
        {
            sim.ClearData();
                sim.stopSimulation();
        }
    }

    public void PlaySimulation()
    {
        simulationsRuning = true;
        startSimulationButton.SetActive(false);
        pauseSimulationButton.SetActive(true);
        playSimulationButton.SetActive(false);
        stopSimulationButton.SetActive(true);

        foreach (var sim in simulationObject)
        {
                sim.startSimulation();
        }
    }

    public void StopSimulation()
    {

        simulationsRuning = false;
        startSimulationButton.SetActive(true);
        pauseSimulationButton.SetActive(false);
        playSimulationButton.SetActive(false);
        stopSimulationButton.SetActive(false);

        foreach (var sim in simulationObject)
        {
            sim.ClearData();
            sim.stopSimulation();
        }
    }
    private void Update()
    {

        if (simulationsRuning)
        {
            elapsedTime += Time.deltaTime;

        }
        UpdateGUI();
    }

    private void UpdateGUI()
    {

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        simulationTimeText.text = $"{minutes:00}:{seconds:00}";
    }

    IEnumerator addACar()
    {
        if(simulationsRuning)
        {
            int randomIndex = Random.Range(0, roadNumPerSim);
            onAddCar.Invoke(randomIndex);
        }
        yield return new WaitForSeconds(soawnInterval);
        StartCoroutine(addACar());
    }

}
