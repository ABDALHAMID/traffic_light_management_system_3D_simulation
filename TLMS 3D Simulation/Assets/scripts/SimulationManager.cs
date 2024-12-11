using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class SimulationManager : MonoBehaviour
{
    public SimulationType simulationType = SimulationType.MIFO;

    public bool simulationRuning = true;

    public Spowner spowner;


    public TextMeshProUGUI activeCars;
    public TextMeshProUGUI finishedCars;
    
    private int activeTrafficLight = 0;
    public DataCollectorMnager[] dataCollectors;



    private void Start()
    {
        
        SetSimulationType(simulationType);
    }

    public void ToggleSimulationType(int val)
    {
        if(val == 0)
            SetSimulationType(SimulationType.MIFO);
        else if(val == 1)
            SetSimulationType(SimulationType.TimeBased);
    }
    public void SetSimulationType(SimulationType newSimulationType)
    {
        StopAllCoroutines();
        simulationType = newSimulationType;
        if(simulationType == SimulationType.MIFO) 
            StartCoroutine(ControlTrafficLights());
        else if(simulationType == SimulationType.TimeBased)
            StartCoroutine(ControlTrafficLightsTimeBased());
    }

    private void Update()
    {
        
            UpdateGUI();
    }

    private void UpdateGUI()
    {
        activeCars.text = spowner.activeCarsCount.ToString();
        finishedCars.text = spowner.finishedCarsCount.ToString();

        
    }

    // Main coroutine to control traffic lights
    private IEnumerator ControlTrafficLights()
    {
        if (simulationRuning)
        {
            UpdateTrafficLightStates();
            
        }
        yield return new WaitForSeconds(2f);
        StartCoroutine(ControlTrafficLights());
    }

    List<RoadTrafficData> importTrafficData()
    {
        List<RoadTrafficData> data = new List<RoadTrafficData>();
        for (int i = 0; i < dataCollectors.Length; i++)
        {

            RoadTrafficData v = new RoadTrafficData(i, dataCollectors[i].trafficLight, dataCollectors[i].numberOfCars);
            data.Add(v);
        }
        return data;
    }

    private IEnumerator ControlTrafficLightsTimeBased()
    {
        if (simulationRuning)
        {
            List<RoadTrafficData> trafficData = importTrafficData();
            for (int i = 0; i < trafficData.Count; i++)
            {
                trafficData[i].trafficLight.SetState("stop");
            }
            activeTrafficLight = (activeTrafficLight + 1) % trafficData.Count;
            trafficData[activeTrafficLight].trafficLight.SetState("move");
            Debug.Log(activeTrafficLight);
        }
        yield return new WaitForSeconds(10f);
        StartCoroutine(ControlTrafficLightsTimeBased());
    }
    private void UpdateTrafficLightStates()
    {
        List<RoadTrafficData> trafficData = importTrafficData();

        int maxCarsIndex = -1;
        int maxCars = int.MinValue;


        for (int i = 0; i < trafficData.Count; i++)
        {
            if (trafficData[i].numberOfCars > maxCars)
            {
                maxCars = trafficData[i].numberOfCars;
                maxCarsIndex = trafficData[i].index;
            }
        }

        // Set all lights to red first
        for (int i = 0; i < trafficData.Count; i++)
        {
            trafficData[i].trafficLight.SetState("stop");
        }
        // Set the light with the most cars to green
        if (maxCarsIndex >= 0)
        {
            trafficData[maxCarsIndex].trafficLight.SetState("move");
        }
    }

    public void ToggelSimulation()
    {
        simulationRuning = !simulationRuning;
    }

    public void ClearData()
    {
        simulationRuning = false;
        spowner.ClearAll();
    }


}

public class RoadTrafficData
{
    public int index;
    public TrafficLightController trafficLight;
    public int numberOfCars;



    public RoadTrafficData(int index, TrafficLightController trafficLight, int numberOfCars)
    {
        this.index = index;
        this.trafficLight = trafficLight;
        this.numberOfCars = numberOfCars;
    }

    public void print()
    {
        Debug.Log("index: " + index + " | statu: " + trafficLight.state.ToString() + " | number of cars: " + numberOfCars);
    }
}

public enum SimulationType
{
    MIFO,
    TimeBased,
}