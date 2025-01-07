using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float cellSize = 1;
    public GameObject obstaclePrefab;
    public GameObject characterPrefab;

    private Grid grid;
    private Grid_A_Star pathfinder;
    private GridCell startCell;
    private GridCell goalCell;
    private GridHeuristic heuristic;
    private System.Collections.Generic.List<GridCell> path;

    void Start()
    {
        Vector3 center = transform.position;
        Vector3 scale = transform.localScale;
        grid = new Grid(cellSize, center, scale, obstaclePrefab, characterPrefab, gameObject);
        pathfinder = new Grid_A_Star(1000, 10f, 1000);
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

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        AStarPathfinder pathfinder = FindObjectOfType<AStarPathfinder>();
        if (pathfinder == null) return;

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
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (path[i] == null || path[i + 1] == null) continue;
                Gizmos.DrawLine(path[i].center, path[i + 1].center);
                DrawCell(path[i]);
            }
            if (path[path.Count - 1] != null)
            {
                DrawCell(path[path.Count - 1]);
            }
        }

        // Draw start point in blue
        if (startCell != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(startCell.center + Vector3.up * 0.1f, cellSize * 0.2f);
        }

        // Draw end point in magenta
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
        Gizmos.DrawCube(cell.center, size);
    }
}
