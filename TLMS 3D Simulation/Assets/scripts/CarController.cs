using System.ComponentModel;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public SimulationManager simulationManager;
    private Rigidbody rb;

    [SerializeField]
    private float speed = 10;

    [SerializeField]
    private float rayLength;

    private bool shouldStop;

    public LayerMask carLayer;
    private int numRays = 5;

    private int destination;
    public float turnSpeed = 50f; // Speed of the turn, adjust as needed
    private float targetRotation;
    private bool turned = false;

    public delegate void CarDestroyed();
    public event CarDestroyed OnCarDestroyed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        destination = Random.RandomRange(1, 4);
    }

    // Update is called once per frame
    void Update()
    {
        if (simulationManager != null)
        {
            if (simulationManager.simulationRuning)
            {
                Move();
            }
        }
        
    }

    private bool CheckForCarAhead()
    {
        for (int i = 0; i < numRays; i++)
        {

            Vector3 rayStart = transform.position + Vector3.up * 0.5f;

            float angleOffset = -15f + (i * 30f / (numRays - 1));

            Vector3 rayDirection = Quaternion.Euler(0, angleOffset, 0) * transform.forward;

            Debug.DrawRay(rayStart, rayDirection * rayLength, Color.blue);

            RaycastHit hit;

            if (Physics.Raycast(rayStart, rayDirection, out hit, rayLength, carLayer))
            {
                return true; 
            }

        }
        return false; 
    }



    void Move()
    {
        if (!shouldStop && !CheckForCarAhead())
        {
            
            transform.Translate(new Vector3(0, 0, 1) * speed);
        }
        else
        {
            transform.Translate(Vector3.zero);
        }

        if (targetRotation != 0)
        {
            float step = turnSpeed * Time.deltaTime;
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation, step);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    private void Turn(float turnAmount)
    {
        targetRotation = transform.eulerAngles.y + turnAmount;
    }

    public void StopCar() 
    {
        shouldStop = true;
    }

    public void startCar()
    {
        shouldStop = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TrafficLightStopPosition"))
        {
            TrafficLightController trafficLightController = other.GetComponentInParent<TrafficLightController>();
            if (trafficLightController != null)
            {
                Vector3 carForward = transform.forward;
                Vector3 lightForward = trafficLightController.transform.forward;

                float angle = Vector3.Angle(carForward, lightForward);

                float directionThreshold = 30f;

                if (angle <= directionThreshold)
                {
                    if (trafficLightController.state == TrafficLightStates.red)
                    {
                        StopCar();
                    }
                    else if (trafficLightController.state == TrafficLightStates.green)
                    {
                        startCar();
                    }
                    else if (trafficLightController.state == TrafficLightStates.yellow)
                    {
                        // Check if the car has passed the stop position
                        Vector3 carPosition = transform.position;
                        Vector3 stopPosition = other.transform.position;

                        // Calculate the direction from the stop position to the car
                        Vector3 toCarDirection = carPosition - stopPosition;

                        // Check if the car is in front of the stop position using a dot product
                        float dotProduct = Vector3.Dot(toCarDirection.normalized, lightForward);

                        if (dotProduct > 0) // Positive dot product means the car is in front
                        {
                            startCar();
                        }
                        else
                        {
                            StopCar();
                        }
                    }
                }
            }
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 carForward = transform.forward;
        Vector3 turnCheckerForward = other.transform.forward;

        float angle = Vector3.Angle(carForward, turnCheckerForward);

        float directionThreshold = 30f;

        if (destination == 1 && other.CompareTag("turnRightChecker") && angle <= directionThreshold && !turned)
        {
            
            Turn(90f); // Turn right by 90 degrees
        }
        else if (destination == 3 && other.CompareTag("turnLeftChecker") && angle <= directionThreshold && !turned)
        {
            turned = true;
            Turn(-90f); // Turn left by 90 degrees
        }
        if (other.CompareTag("destroyCarPosition"))
        {
            DestroyCar();
        }
    }

    public void DestroyCar()
    {
        // Trigger the destruction event
        OnCarDestroyed?.Invoke();

        // Destroy the car GameObject
        Destroy(gameObject);
    }




}
