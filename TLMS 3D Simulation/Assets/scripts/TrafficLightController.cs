using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public enum TrafficLightStates
{
    red,
    green,
    yellow,
}
public class TrafficLightController : MonoBehaviour
{
    public SimulationManager simulationManager;
    public TrafficLightStates state;
    public RawImage UIImage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (simulationManager != null) {
            if (simulationManager.simulationRuning) {
                HandleState();
            }
        }
    }

    void HandleState()
    {
        if (state == TrafficLightStates.red)
        {
            UIImage.color = Color.red;
        }
        else if (state == TrafficLightStates.green)
        {
            UIImage.color = Color.green;
        }
        else if (state == TrafficLightStates.yellow)
        {
            UIImage.color = Color.yellow;
        }
    }


    public void ChangeState(TrafficLightStates state)
    {
        this.state = state;
    }

    public void SetState(string v)
    {
        switch (v)
        {
            case "stop":
                Debug.Log("stop");
                if (state != TrafficLightStates.green)
                    break;
                StartCoroutine(StartStoping());
                break;
            case "move":
                Debug.Log("Start");
                state = TrafficLightStates.green;
                break;
        }
    }

    private IEnumerator StartStoping()
    {
        state = TrafficLightStates.yellow;
        yield return new WaitForSeconds(3f);
        state = TrafficLightStates.red;
    }
}
