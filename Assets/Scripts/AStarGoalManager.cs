using UnityEngine;

public class AStarGoalManager : MonoBehaviour
{
    public float bufferRadius = 0.1f;
    private GridManager gridManager;
    private AStarPathfinder pathfinder;

    void Start()
    {
        // Wait for PathfindingManager to initialize
        if (PathfindingManager.Instance == null)
        {
            Debug.LogError("PathfindingManager not found in scene!");
            return;
        }

        gridManager = PathfindingManager.Instance.gridManager;
        pathfinder = PathfindingManager.Instance.GetPathfinder();
        
        if (gridManager == null)
            Debug.LogError("GridManager not found!");
        if (pathfinder == null)
            Debug.LogError("AStarPathfinder not found!");
    }

    public void AssignNewGoal(AStarAgent agent)
    {
        if (gridManager == null || pathfinder == null) return;

        // Keep trying until we find a valid path
        for (int attempts = 0; attempts < 10; attempts++)
        {
            // Pick a random non-occupied node
            int randomGoalIndex = Random.Range(0, gridManager.GetGrid().getNumNodes());
            GridCell goalCell = gridManager.GetGrid().getNode(randomGoalIndex);
            
            if (goalCell != null && !goalCell.IsOccupied)
            {
                var path = pathfinder.FindPath(agent.currentNodeIndex, randomGoalIndex);
                if (path.Count > 0)
                {
                    agent.targetNodeIndex = randomGoalIndex;
                    agent.SetPath(pathfinder.GetPathCells());
                    Debug.Log($"New goal assigned: Node {randomGoalIndex}");
                    return;
                }
            }
        }
        Debug.LogWarning("Failed to find valid goal after 10 attempts");
    }

    public bool IsGoalReached(AStarAgent agent)
    {
        return agent.IsPathComplete();
    }
} 