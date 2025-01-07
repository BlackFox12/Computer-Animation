using UnityEngine;
using System.Collections.Generic;
using PathFinding;

public class AStarPathfinder : MonoBehaviour
{
    private Grid grid;
    private A_Star<GridCell, CellConnection, GridConnections, Grid, GridHeuristic> aStar;
    private List<GridCell> lastPathCells;
    private List<GridCell> visitedNodes = new List<GridCell>();

    void Start()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }

        grid = gridManager.GetGrid();
        if (grid == null)
        {
            Debug.LogError("Grid not initialized in GridManager!");
            return;
        }

        aStar = new A_Star<GridCell, CellConnection, GridConnections, Grid, GridHeuristic>(1000, 10f, 50);
        Debug.Log("AStarPathfinder initialized successfully");
    }

    public List<Vector3> FindPath(int startIndex, int goalIndex)
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
        foreach (var cell in lastPathCells)
        {
            path.Add(cell.center);
        }

        Debug.Log($"Path found from {startIndex} to {goalIndex} with {path.Count} waypoints");
        return path;
    }

    public List<GridCell> GetPathCells()
    {
        return lastPathCells ?? new List<GridCell>();
    }

    public List<GridCell> GetVisitedNodes()
    {
        return visitedNodes;
    }

    public bool IsInOpenSet(GridCell cell)
    {
        return aStar != null && aStar.IsInOpenSet(cell);
    }

    public bool IsInClosedSet(GridCell cell)
    {
        return aStar != null && aStar.IsInClosedSet(cell);
    }
} 