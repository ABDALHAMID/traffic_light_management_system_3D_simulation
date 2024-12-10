using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public Transform[] waypoints;
    public float transitionSpeed = 2f;

    private int currentWaypointIndex = 0;
    private bool isTransitioning = false;

    void Update()
    {
        if (isTransitioning)
        {
            SmoothMoveToWaypoint(waypoints[currentWaypointIndex]);
        }
    }

    public void SwitchToWaypoint()
    {
        int index = (currentWaypointIndex + 1) % waypoints.Length ;
        if (index >= 0 && index < waypoints.Length)
        {
            currentWaypointIndex = index;
            isTransitioning = true;
        }
        else
        {
            Debug.LogWarning("Waypoint index out of range!");
        }
    }

    private void SmoothMoveToWaypoint(Transform targetWaypoint)
    {
        // Smoothly interpolate the camera's position
        transform.position = Vector3.Lerp(transform.position, targetWaypoint.position, Time.deltaTime * transitionSpeed);

        // Smoothly interpolate the camera's rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetWaypoint.rotation, Time.deltaTime * transitionSpeed);

        // Stop transitioning when close enough to the target
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.01f &&
            Quaternion.Angle(transform.rotation, targetWaypoint.rotation) < 0.1f)
        {
            isTransitioning = false;
        }
    }
}
