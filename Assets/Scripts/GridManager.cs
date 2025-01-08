using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public float cellSize = 1;
    public GameObject obstaclePrefab;
    public bool showPathfinding = false;

    private Grid grid;
    private Grid_A_Star pathfinder;
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
        pathfinder = new Grid_A_Star(1000, 10f, 1000);
    }

    void Update()
    {
        if (showPathfinding)
        {
            GenerateRandomPath();
            showPathfinding = false; // Reset flag after generating path
        }
    }

    private void GenerateRandomPath()
    {
        if (grid == null || grid.nodes.Count == 0) return;

        // Find random start and goal cells
        startCell = null;
        goalCell = null;
        int attempts = 0;
        float minDistance = Mathf.Max(transform.localScale.x, transform.localScale.z) * 7f; // At least 60% of grid size
        
        while ((startCell == null || goalCell == null) && attempts < 20)
        {
            int startIndex = Random.Range(0, grid.getNumNodes());
            int goalIndex = Random.Range(0, grid.getNumNodes());
            
            GridCell potentialStart = grid.getNode(startIndex);
            GridCell potentialGoal = grid.getNode(goalIndex);

            if (potentialStart != null && !potentialStart.IsOccupied && startCell == null)
                startCell = potentialStart;
                
            if (potentialGoal != null && !potentialGoal.IsOccupied && goalCell == null && 
                potentialGoal != startCell && 
                (startCell == null || Vector3.Distance(startCell.center, potentialGoal.center) >= minDistance))
            {
                goalCell = potentialGoal;
            }

            attempts++;
        }

        if (startCell == null || goalCell == null)
        {
            Debug.LogWarning("Could not find valid start and goal cells");
            return;
        }

        // Find path
        int found = 0;
        heuristic = new GridHeuristic(goalCell);
        path = pathfinder.findpath(grid, startCell, goalCell, heuristic, ref found);
        visitedNodes = pathfinder.getVisitedNodes();
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || grid == null) return;

        // Draw visited nodes
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
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (path[i] == null || path[i + 1] == null) continue;
                Gizmos.DrawLine(path[i].center + Vector3.up * 0.1f, 
                              path[i + 1].center + Vector3.up * 0.1f);
                DrawCell(path[i]);
            }
            DrawCell(path[path.Count - 1]);
        }

        // Draw start point in blue
        if (startCell != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(startCell.center + Vector3.up * 0.1f, cellSize * 0.2f);
        }

        // Draw end point in purple
        if (goalCell != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(goalCell.center + Vector3.up * 0.1f, cellSize * 0.2f);
        }
    }

    private void DrawCell(GridCell cell)
    {
        if (cell == null) return;
        Vector3 size = new Vector3(cellSize, 0.1f, cellSize);
        Gizmos.DrawCube(cell.center + Vector3.up * 0.05f, size);
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
            float distance = Vector3.Distance(position, cell.center);
            if (distance < minDistance && !cell.IsOccupied)
            {
                minDistance = distance;
                nearestCell = cell;
            }
        }

        return nearestCell;
    }
}
