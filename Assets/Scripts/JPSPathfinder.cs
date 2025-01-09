using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;

public class JPSPathfinder : MonoBehaviour
{
    protected Grid grid;
    protected JumpPointSearch<GridCell, CellConnection, GridConnections, Grid, GridHeuristic> jps;
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
        while (JPSPathfindingManager.Instance == null || !JPSPathfindingManager.Instance.IsInitialized)
        {
            Debug.Log($"{GetType().Name} waiting for JPSPathfindingManager...");
            yield return new WaitForSeconds(0.1f);
        }

        JPSGridManager gridManager = JPSPathfindingManager.Instance.gridManager;
        while (gridManager == null || gridManager.GetGrid() == null)
        {
            Debug.Log($"{GetType().Name} waiting for Grid...");
            yield return new WaitForSeconds(0.1f);
            gridManager = JPSPathfindingManager.Instance.gridManager;
        }

        grid = gridManager.GetGrid();
        InitializePathfinder();
        isInitialized = true;
        Debug.Log($"{GetType().Name} initialized successfully");
    }

    protected virtual void InitializePathfinder()
    {
        jps = new JumpPointSearch<GridCell, CellConnection, GridConnections, Grid, GridHeuristic>(1000, 10f, 50);
    }

    public virtual List<Vector3> FindPath(int startIndex, int goalIndex)
    {
        if (!IsInitialized())
        {
            Debug.LogWarning("JPSPathfinder not yet initialized!");
            return new List<Vector3>();
        }

        try
        {
            GridCell startCell = grid.getNode(startIndex);
            GridCell goalCell = grid.getNode(goalIndex);

            if (startCell == null || goalCell == null || startCell.IsOccupied || goalCell.IsOccupied)
            {
                Debug.LogWarning($"Invalid path request: start={startIndex}, goal={goalIndex}");
                return new List<Vector3>();
            }

            int found = 0;
            var heuristic = new GridHeuristic(goalCell);
            lastPathCells = jps.findpath(grid, startCell, goalCell, heuristic, ref found);
            visitedNodes = jps.getVisitedNodes();

            List<Vector3> path = new List<Vector3>();
            if (lastPathCells != null)
            {
                foreach (var cell in lastPathCells)
                {
                    if (cell != null)
                        path.Add(cell.center);
                }
            }

            Debug.Log($"JPS Path found from {startIndex} to {goalIndex} with {path.Count} waypoints");
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
        return jps != null && jps.IsInOpenSet(cell);
    }

    public virtual bool IsInClosedSet(GridCell cell)
    {
        return jps != null && jps.IsInClosedSet(cell);
    }

    public bool IsInitialized()
    {
        return isInitialized && grid != null && jps != null;
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