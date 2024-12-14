using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SceneSimulationManager : MonoBehaviour
{
    public int simulationId = 0;
    private int currentENV = 0;
    public TextMeshProUGUI simulationTimeText;
    public GameObject startSimulationButton, pauseSimulationButton, playSimulationButton, stopSimulationButton, addSimulationButton, removeSimuationButton;

    private float elapsedTime;

    private Vector3[] simulationsPostion = {new Vector3(0,0,0),
                                            new Vector3(1000, 10000, 1000),
                                            new Vector3(2000, 20000, 2000),
                                            new Vector3(3000, 30000, 3000)};

    public bool simulationsRuning { get; private set; }
    public int NumberOfSimulation { get; private set; } = 0;


    public GameObject simulationObject;

    public List<GameObject> Simulations = new List<GameObject>();
    private void Start()
    {
        startSimulationButton.SetActive(true);
        pauseSimulationButton.SetActive(false);
        playSimulationButton.SetActive(false);
        stopSimulationButton.SetActive(false);
        addSimulationButton.SetActive(true);
        removeSimuationButton.SetActive(true);
    }


    public void StartSimulations()
    {
        elapsedTime = 0f;
        simulationsRuning = true;
        startSimulationButton.SetActive(false);
        pauseSimulationButton.SetActive(true);
        playSimulationButton.SetActive(false);
        stopSimulationButton.SetActive(true);
        addSimulationButton.SetActive(false);
        removeSimuationButton.SetActive(false);

        foreach (var sim in Simulations)
        {
            if (sim.activeSelf)
                sim.GetComponentInChildren<SimulationManager>().startSimulation();
        }
    }

    public void PauseSimulation()
    {
        simulationsRuning = false;
        startSimulationButton.SetActive(false);
        pauseSimulationButton.SetActive(false);
        playSimulationButton.SetActive(true);
        stopSimulationButton.SetActive(true);
        addSimulationButton.SetActive(false);
        removeSimuationButton.SetActive(false);

        foreach (var sim in Simulations)
        {
            if (sim.activeSelf)
                sim.GetComponentInChildren<SimulationManager>().stopSimulation();
        }
    }

    public void PlaySimulation()
    {
        simulationsRuning = true;
        startSimulationButton.SetActive(false);
        pauseSimulationButton.SetActive(true);
        playSimulationButton.SetActive(false);
        stopSimulationButton.SetActive(true);
        addSimulationButton.SetActive(false);
        removeSimuationButton.SetActive(false);

        foreach (var sim in Simulations)
        {
            if (sim.activeSelf)
                sim.GetComponentInChildren<SimulationManager>().startSimulation();
        }
    }

    public void StopSimulation()
    {
        simulationsRuning = false;
        startSimulationButton.SetActive(true);
        pauseSimulationButton.SetActive(false);
        playSimulationButton.SetActive(false);
        stopSimulationButton.SetActive(false);
        addSimulationButton.SetActive(true);
        removeSimuationButton.SetActive(true);

        foreach (var sim in Simulations)
        {
            if (sim.activeSelf)
                sim.GetComponentInChildren<SimulationManager>().resetSimulation();
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


    public void AddSimulation()
    {
        if (NumberOfSimulation == 4)
        {
            Debug.LogWarning("number of simulation is max");
            return;
        }
        NumberOfSimulation++;
        GameObject simulation = Instantiate(simulationObject, simulationsPostion[NumberOfSimulation - 1], quaternion.identity);

        currentENV++;
        simulation.GetComponentInChildren<SimulationManager>().roundaboutID = currentENV;

        Simulations.Add(simulation);
        SetCameras();
    }


    private void SetCameras()
    {
        if (NumberOfSimulation == 1)
        {
            Simulations[0].GetComponentInChildren<Camera>().rect = new Rect(0, 0, 1, 1);
        }
        else if (NumberOfSimulation == 2) 
        {
            Simulations[0].GetComponentInChildren<Camera>().rect = new Rect(0, 0, .5f, 1);
            Simulations[1].GetComponentInChildren<Camera>().rect = new Rect(.5f, 0, .5f, 1);
        }
        else if(NumberOfSimulation == 3)
        {
            Simulations[0].GetComponentInChildren<Camera>().rect = new Rect(0, 0, .5f, 1);
            Simulations[1].GetComponentInChildren<Camera>().rect = new Rect(.5f, .5f, .5f, .5f);
            Simulations[2].GetComponentInChildren<Camera>().rect = new Rect(.5f, 0, .5f, .5f);
        }
        else
        {
            Simulations[0].GetComponentInChildren<Camera>().rect = new Rect(0, .5f, .5f, .5f);
            Simulations[1].GetComponentInChildren<Camera>().rect = new Rect(0, 0, .5f, .5f);
            Simulations[2].GetComponentInChildren<Camera>().rect = new Rect(.5f, .5f, .5f, .5f);
            Simulations[3].GetComponentInChildren<Camera>().rect = new Rect(.5f, 0, .5f, .5f);
        }
    }

    public void RemoveSimulation()
    {
        var simulation = Simulations[NumberOfSimulation - 1];
        Simulations.Remove(simulation);
        simulation.GetComponentInChildren<SimulationManager>().ClearData();
        Destroy(simulation);
        NumberOfSimulation--;
        SetCameras();
    }

}
