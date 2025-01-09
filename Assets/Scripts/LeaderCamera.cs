using UnityEngine;

public class LeaderCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform leader;

    [Header("Camera Position")]
    public float heightAboveLeader = 10f;
    public float distanceBehindLeader = 5f;
    
    [Header("Camera Settings")]
    public float smoothSpeed = 0.125f;
    public float lookAheadDistance = 2f; // How far ahead of the leader to look
    
    private void LateUpdate()
    {
        if (leader == null)
        {
            // Try to find the leader if not assigned
            ReynoldsAgent[] agents = FindObjectsOfType<ReynoldsAgent>();
            foreach (var agent in agents)
            {
                if (agent.isLeader)
                {
                    leader = agent.transform;
                    break;
                }
            }
            if (leader == null) return;
        }

        // Calculate desired position
        Vector3 targetPosition = leader.position;
        targetPosition -= leader.forward * distanceBehindLeader; // Move behind
        targetPosition += Vector3.up * heightAboveLeader; // Move up

        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        // Calculate look target (slightly ahead of the leader)
        Vector3 lookTarget = leader.position + leader.forward * lookAheadDistance;
        
        // Make camera look at target
        transform.LookAt(lookTarget);
    }

    // Optional: Add methods to dynamically adjust camera parameters
    public void SetHeight(float height)
    {
        heightAboveLeader = height;
    }

    public void SetDistance(float distance)
    {
        distanceBehindLeader = distance;
    }
} 