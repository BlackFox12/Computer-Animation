using UnityEngine;
using System.Collections;

public class AStarGoalManager : MonoBehaviour
{
    public float bufferRadius = 0.1f;
    private GridManager gridManager;
    private AStarPathfinder pathfinder;
    private bool isInitialized = false;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        // Wait for PathfindingManager to be initialized
        while (PathfindingManager.Instance == null || !PathfindingManager.Instance.IsInitialized)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Wait for components to be available
        while (true)
        {
            gridManager = PathfindingManager.Instance.gridManager;
            pathfinder = PathfindingManager.Instance.GetPathfinder();

            if (gridManager != null && pathfinder != null && pathfinder.IsInitialized())
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        isInitialized = true;
        Debug.Log("AStarGoalManager initialized successfully");
    }

    public void AssignNewGoal(AStarAgent agent)
    {
        if (!isInitialized || agent == null)
        {
            Debug.LogWarning("Cannot assign goal - system not initialized or agent is null");
            return;
        }

        try
        {
            // Update the agent's current node index based on its position
            GridCell currentCell = gridManager.FindNearestCell(agent.transform.position);
            if (currentCell == null)
            {
                Debug.LogWarning("Could not find valid cell for agent's current position");
                return;
            }
            agent.currentNodeIndex = currentCell.getId();

            // Keep trying until we find a valid path
            for (int attempts = 0; attempts < 10; attempts++)
            {
                // Pick a random non-occupied node
                int randomGoalIndex = Random.Range(0, gridManager.GetGrid().getNumNodes());
                GridCell goalCell = gridManager.GetGrid().getNode(randomGoalIndex);
                
                if (goalCell != null && !goalCell.IsOccupied && randomGoalIndex != agent.currentNodeIndex)
                {
                    var path = pathfinder.FindPath(agent.currentNodeIndex, randomGoalIndex);
                    if (path != null && path.Count > 0)
                    {
                        agent.targetNodeIndex = randomGoalIndex;
                        agent.SetPath(pathfinder.GetPathCells());
                        Debug.Log($"New goal assigned: From node {agent.currentNodeIndex} to {randomGoalIndex}");
                        return;
                    }
                }
            }
            Debug.LogWarning("Failed to find valid goal after 10 attempts");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in AssignNewGoal: {e.Message}");
        }
    }

    public bool IsGoalReached(AStarAgent agent)
    {
        return agent != null && agent.IsPathComplete();
    }
} 