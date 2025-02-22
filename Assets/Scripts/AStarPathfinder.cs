using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding;

public class AStarPathfinder : MonoBehaviour
{
    protected Grid grid;
    protected A_Star<GridCell, CellConnection, GridConnections, Grid, GridHeuristic> aStar;
    protected List<GridCell> lastPathCells;
    protected List<GridCell> visitedNodes = new List<GridCell>();
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
            Debug.Log($"{GetType().Name} waiting for PathfindingManager...");
            yield return new WaitForSeconds(0.1f);
        }

        GridManager gridManager = PathfindingManager.Instance.gridManager;
        while (gridManager == null || gridManager.GetGrid() == null)
        {
            Debug.Log($"{GetType().Name} waiting for Grid...");
            yield return new WaitForSeconds(0.1f);
            gridManager = PathfindingManager.Instance.gridManager;
        }

        grid = gridManager.GetGrid();
        InitializePathfinder();
        isInitialized = true;
        Debug.Log($"{GetType().Name} initialized successfully");
    }

    protected virtual void InitializePathfinder()
    {
        aStar = new A_Star<GridCell, CellConnection, GridConnections, Grid, GridHeuristic>(1000, 10f, 50);
    }

    public virtual List<Vector3> FindPath(int startIndex, int goalIndex)
    {
        if (!IsInitialized())
        {
            Debug.LogWarning("AStarPathfinder not yet initialized!");
            return new List<Vector3>();
        }

        try
        {
            // Get the start and goal nodes by index
            GridCell startCell = grid.getNode(startIndex);
            GridCell goalCell = grid.getNode(goalIndex);

            // Validate start and goal nodes
            if (startCell == null || goalCell == null || startCell.IsOccupied || goalCell.IsOccupied)
            {
                Debug.LogWarning($"Invalid path request: start={startIndex}, goal={goalIndex}");
                return new List<Vector3>();
            }

            int found = 0;
            var heuristic = new GridHeuristic(goalCell);
            lastPathCells = aStar.findpath(grid, startCell, goalCell, heuristic, ref found);
            visitedNodes = aStar.getVisitedNodes();

            // Convert path of cells to path of world positions
            List<Vector3> path = new List<Vector3>();
            if (lastPathCells != null)
            {
                foreach (var cell in lastPathCells)
                {
                    if (cell != null)
                        path.Add(cell.center);
                }
            }

            Debug.Log($"Path found from {startIndex} to {goalIndex} with {path.Count} waypoints");
            return path;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in FindPath: {e.Message}");
            return new List<Vector3>();
        }
    }

    public virtual bool IsInOpenSet(GridCell cell)
    {
        return aStar != null && aStar.IsInOpenSet(cell);
    }

    public virtual bool IsInClosedSet(GridCell cell)
    {
        return aStar != null && aStar.IsInClosedSet(cell);
    }

    public bool IsInitialized()
    {
        return isInitialized && grid != null && aStar != null;
    }

    public List<GridCell> GetPathCells()
    {
        return lastPathCells ?? new List<GridCell>();
    }

    public List<GridCell> GetVisitedNodes()
    {
        return visitedNodes;
    }
} 