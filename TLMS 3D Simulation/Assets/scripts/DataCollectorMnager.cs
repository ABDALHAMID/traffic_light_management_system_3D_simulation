using UnityEngine;

public class DataCollectorMnager : MonoBehaviour
{
    [SerializeField]
    public TrafficLightController trafficLight;
    [SerializeField]
    public int numberOfCars;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            numberOfCars++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            numberOfCars--;
        }
    }
}
