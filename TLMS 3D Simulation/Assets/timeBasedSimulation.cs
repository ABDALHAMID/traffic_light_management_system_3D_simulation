using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class timeBasedSimulation : MonoBehaviour
{
    public bool simulationRuning = true;

    public Spowner spowner;

    public TextMeshProUGUI totalCars;
    public TextMeshProUGUI activeCars;
    public TextMeshProUGUI finishedCars;

    public DataCollectorMnager[] dataCollectors;

    private int activeTrafficLight = 0;
    void Start()
    {
        StartCoroutine(ControlTrafficLights());
    }

    private void Update()
    {
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        totalCars.text = spowner.totalCarsCount.ToString();
        activeCars.text = spowner.activeCarsCount.ToString();
        finishedCars.text = spowner.finishedCarsCount.ToString();
    }

    // Main coroutine to control traffic lights
    private IEnumerator ControlTrafficLights()
    {
        if (simulationRuning)
        {
            List<RoadTrafficData> trafficData = importTrafficData();
            for (int i = 0; i < trafficData.Count; i++)
            {
                trafficData[i].trafficLight.SetState("stop");
            }
            int nextIndex = activeTrafficLight = activeTrafficLight++ * (activeTrafficLight % trafficData.Count);
            trafficData[nextIndex].trafficLight.SetState("move");
            Debug.Log(nextIndex);
        }
        yield return new WaitForSeconds(10f);
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

}
