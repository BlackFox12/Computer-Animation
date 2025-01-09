using UnityEngine;
using System.Collections.Generic;
using PathFinding;

public class JPSGridManager : MonoBehaviour
{
    public float cellSize = 1;
    public GameObject obstaclePrefab;
    public bool showPathfinding = false;

    private Grid grid;
    private JumpPointSearch<GridCell, CellConnection, GridConnections, Grid, GridHeuristic> pathfinder;
    private GridCell startCell;
    private GridCell goalCell;
    private GridHeuristic heuristic;
    private List<GridCell> path;
    private List<GridCell> visitedNodes;

    void Start()
    {
        Vector3 center = transform.position;
        Vector3 scale = transform.localScale;
        grid = new Grid(cellSize, center, scale, obstaclePrefab, gameObject);
        pathfinder = new JumpPointSearch<GridCell, CellConnection, GridConnections, Grid, GridHeuristic>(1000, 10f, 1000);
    }

    void Update()
    {
        if (showPathfinding)
        {
            GenerateRandomPath();
            showPathfinding = false;
        }
    }

    private void GenerateRandomPath()
    {
        if (grid == null || grid.nodes.Count == 0) return;

        startCell = null;
        goalCell = null;
        int attempts = 0;
        float minDistance = Mathf.Max(transform.localScale.x, transform.localScale.z) * 0.7f; // 70% of grid size
        
        while ((startCell == null || goalCell == null) && attempts < 20)
        {
            int startIndex = Random.Range(0, grid.getNumNodes());
            int goalIndex = Random.Range(0, grid.getNumNodes());
            
            GridCell potentialStart = grid.getNode(startIndex);
            GridCell potentialGoal = grid.getNode(goalIndex);

            // Check for valid start position
            if (potentialStart != null && !potentialStart.IsOccupied && startCell == null)
            {
                // For JPS, prefer start positions with clear lines of sight
                if (HasClearNeighbors(potentialStart))
                {
                    startCell = potentialStart;
                }
            }
                
            // Check for valid goal position
            if (potentialGoal != null && !potentialGoal.IsOccupied && goalCell == null && 
                potentialGoal != startCell && 
                (startCell == null || Vector3.Distance(startCell.center, potentialGoal.center) >= minDistance))
            {
                // For JPS, prefer goal positions with clear lines of sight
                if (HasClearNeighbors(potentialGoal))
                {
                    goalCell = potentialGoal;
                }
            }

            attempts++;
        }

        if (startCell == null || goalCell == null)
        {
            Debug.LogWarning("Could not find valid start and goal cells for JPS");
            return;
        }

        // Find path using JPS
        int found = 0;
        heuristic = new GridHeuristic(goalCell);
        path = pathfinder.findpath(grid, startCell, goalCell, heuristic, ref found);
        visitedNodes = pathfinder.getVisitedNodes();
    }

    // Helper method for JPS to check if a cell has clear neighboring paths
    private bool HasClearNeighbors(GridCell cell)
    {
        int clearNeighbors = 0;
        var connections = grid.getConnections(cell);
        
        foreach (var connection in connections.connections)
        {
            GridCell neighbor = connection.getToNode() as GridCell;
            if (neighbor != null && !neighbor.IsOccupied)
            {
                clearNeighbors++;
            }
        }

        // Return true if the cell has at least 3 clear neighbors
        return clearNeighbors >= 3;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || grid == null) return;

        // Draw visited nodes with JPS-specific coloring
        if (visitedNodes != null)
        {
            foreach (GridCell cell in visitedNodes)
            {
                if (cell == null) continue;

                // Draw OPEN nodes in bright green (jump points)
                if (pathfinder.IsInOpenSet(cell))
                {
                    Gizmos.color = new Color(0, 1, 0.5f, 0.4f);
                    DrawCell(cell);
                }
                // Draw CLOSED nodes in dark red (explored areas)
                else if (pathfinder.IsInClosedSet(cell))
                {
                    Gizmos.color = new Color(0.8f, 0, 0, 0.3f);
                    DrawCell(cell);
                }
            }
        }

        // Draw path in bright yellow
        if (path != null && path.Count > 0)
        {
            Gizmos.color = new Color(1, 1, 0, 0.8f);
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (path[i] == null || path[i + 1] == null) continue;
                // Draw thicker lines for JPS paths
                DrawThickLine(path[i].center, path[i + 1].center, 0.2f);
                DrawCell(path[i]);
            }
            DrawCell(path[path.Count - 1]);
        }

        // Draw start point in bright blue
        if (startCell != null)
        {
            Gizmos.color = new Color(0, 0.5f, 1, 0.8f);
            Gizmos.DrawSphere(startCell.center + Vector3.up * 0.1f, cellSize * 0.25f);
        }

        // Draw end point in bright purple
        if (goalCell != null)
        {
            Gizmos.color = new Color(1, 0, 1, 0.8f);
            Gizmos.DrawSphere(goalCell.center + Vector3.up * 0.1f, cellSize * 0.25f);
        }
    }

    private void DrawCell(GridCell cell)
    {
        if (cell == null) return;
        Vector3 size = new Vector3(cellSize, 0.1f, cellSize);
        Gizmos.DrawCube(cell.center + Vector3.up * 0.05f, size);
    }

    private void DrawThickLine(Vector3 start, Vector3 end, float thickness)
    {
        Vector3 up = Vector3.up * thickness;
        start += Vector3.up * 0.1f;
        end += Vector3.up * 0.1f;
        
        // Draw multiple lines to create thickness
        Gizmos.DrawLine(start, end);
        Gizmos.DrawLine(start + up, end + up);
        Gizmos.DrawLine(start - up, end - up);
    }

    public Grid GetGrid()
    {
        return grid;
    }

    public GridCell FindNearestCell(Vector3 position)
    {
        float minDistance = float.MaxValue;
        GridCell nearestCell = null;

        foreach (GridCell cell in grid.nodes)
        {
            if (cell.IsOccupied) continue; // JPS works better with clear paths

            float distance = Vector3.Distance(position, cell.center);
            if (distance < minDistance)
            {
                // For JPS, prefer cells with clear neighbors
                if (HasClearNeighbors(cell))
                {
                    minDistance = distance;
                    nearestCell = cell;
                }
            }
        }

        return nearestCell;
    }
} 