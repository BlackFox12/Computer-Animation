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
    private AStarPathfinder pathfinder;

    void Start()
    {
        animator = GetComponent<Animator>();
        gridManager = FindObjectOfType<GridManager>();
        pathfinder = PathfindingManager.Instance?.GetPathfinder();
        
        // Set initial node index based on spawn position
        if (gridManager != null)
        {
            GridCell nearestCell = gridManager.FindNearestCell(transform.position);
            if (nearestCell != null)
            {
                currentNodeIndex = nearestCell.getId();
                targetPosition = transform.position;
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

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || pathfinder == null) return;

        // Draw visited nodes
        var visitedNodes = pathfinder.GetVisitedNodes();
        if (visitedNodes != null)
        {
            foreach (GridCell cell in visitedNodes)
            {
                if (cell == null) continue;

                // Draw OPEN nodes in green
                if (pathfinder.IsInOpenSet(cell))
                {
                    Gizmos.color = new Color(0, 1, 0, 0.3f);
                    DrawCell(cell);
                }
                // Draw CLOSED nodes in red
                else if (pathfinder.IsInClosedSet(cell))
                {
                    Gizmos.color = new Color(1, 0, 0, 0.3f);
                    DrawCell(cell);
                }
            }
        }

        // Draw path in yellow
        if (pathNodes != null && pathNodes.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < pathNodes.Count - 1; i++)
            {
                if (pathNodes[i] == null || pathNodes[i + 1] == null) continue;
                Gizmos.DrawLine(pathNodes[i].center + Vector3.up * 0.1f, 
                              pathNodes[i + 1].center + Vector3.up * 0.1f);
                DrawCell(pathNodes[i]);
            }
            if (pathNodes[pathNodes.Count - 1] != null)
            {
                DrawCell(pathNodes[pathNodes.Count - 1]);
            }
        }

        // Draw goal in teal
        if (targetNodeIndex >= 0 && gridManager != null)
        {
            GridCell goalCell = gridManager.GetGrid()?.getNode(targetNodeIndex);
            if (goalCell != null)
            {
                Gizmos.color = new Color(0, 1, 1, 0.5f); // Teal
                Gizmos.DrawWireSphere(goalCell.center + Vector3.up * 0.1f, radius);
            }
        }
    }

    private void DrawCell(GridCell cell)
    {
        if (cell == null) return;
        Vector3 size = new Vector3(1f, 0.1f, 1f); // Assuming 1 unit cell size
        Gizmos.DrawCube(cell.center + Vector3.up * 0.05f, size);
    }
} 