using UnityEngine;
using System.Collections.Generic;

public class AStarAgent : MonoBehaviour
{
    public float velocity = 5f;
    public float radius = 0.5f;
    public int currentNodeIndex;
    public int targetNodeIndex;
    
    private List<GridCell> pathNodes = new List<GridCell>();
    private int currentPathIndex = 0;
    private Animator animator;
    private Vector3 targetPosition;
    private GridManager gridManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        gridManager = FindObjectOfType<GridManager>();
        // Set initial node index based on spawn position
        if (gridManager != null)
        {
            GridCell nearestCell = gridManager.FindNearestCell(transform.position);
            if (nearestCell != null)
            {
                currentNodeIndex = nearestCell.getId();
                targetPosition = transform.position; // Initialize target position
            }
            else
            {
                Debug.LogError("Could not find valid starting cell for agent!");
            }
        }
    }

    public void SetPath(List<GridCell> newPath)
    {
        if (newPath == null || newPath.Count == 0)
        {
            Debug.LogWarning("Attempted to set empty path for agent");
            return;
        }

        pathNodes = newPath;
        currentPathIndex = 0;
        targetPosition = pathNodes[currentPathIndex].center;
        Debug.Log($"New path set with {newPath.Count} nodes. First target: {targetPosition}");
    }

    public Vector3 GetTargetPosition()
    {
        if (pathNodes != null && currentPathIndex < pathNodes.Count)
        {
            return pathNodes[currentPathIndex].center;
        }
        return transform.position;
    }

    public bool IsPathComplete()
    {
        return pathNodes == null || currentPathIndex >= pathNodes.Count;
    }

    public void UpdatePosition(float deltaTime)
    {
        if (IsPathComplete()) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // Move towards target
        if (distanceToTarget > 0.1f)
        {
            transform.position += direction * velocity * deltaTime;

            // Rotate character to face movement direction
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, deltaTime * 5f);
            }
        }
        // Reached current waypoint
        else
        {
            currentPathIndex++;
            if (currentPathIndex < pathNodes.Count)
            {
                targetPosition = pathNodes[currentPathIndex].center;
                Debug.Log($"Moving to next waypoint: {targetPosition}");
            }
        }

        // Update animation parameters
        if (animator != null)
        {
            Vector3 localDirection = transform.InverseTransformDirection(direction);
            animator.SetFloat("Vel_x", localDirection.x);
            animator.SetFloat("Vel_z", localDirection.z);
            animator.SetFloat("Speed_multiplier", distanceToTarget > 0.1f ? 1f : 0f);
            animator.SetBool("isIdle", distanceToTarget <= 0.1f);
        }
    }
} 