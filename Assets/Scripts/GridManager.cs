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
}
