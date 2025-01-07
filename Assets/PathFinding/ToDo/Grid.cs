using UnityEngine;
using System.Collections.Generic;
using PathFinding;

public class Grid : FiniteGraph<GridCell, CellConnection, GridConnections>
{
    protected Vector3 center;
    protected Vector3 scale;
    protected float sizeOfCell;
    protected float gridHeight;
    protected int numRows;
    protected int numColumns;

    public GameObject obstaclePrefab;
    public GameObject characterPrefab;

    public Grid(float cellSize, Vector3 center, Vector3 scale, GameObject obstaclePrefab = null, GameObject charPrefab = null, GameObject existingPlane = null)
    {
        this.center = center;
        this.scale = scale;
        sizeOfCell = cellSize;
        gridHeight = center.y;
        characterPrefab = charPrefab;

        // Calculate number of rows and columns based on scale
        numRows = Mathf.CeilToInt(scale.z * 10 / sizeOfCell);    // Plane is 10 units by default
        numColumns = Mathf.CeilToInt(scale.x * 10 / sizeOfCell); // Plane is 10 units by default

        nodes = new List<GridCell>();
        connections = new List<GridConnections>();
        this.obstaclePrefab = obstaclePrefab;

        GenerateGrid();
    }

    protected void GenerateGrid()
    {
        // Calculate the total width and height of the plane
        float totalWidth = scale.x * 10;  // Plane is 10 units by default
        float totalDepth = scale.z * 10;  // Plane is 10 units by default

        // Calculate start positions at the edge of the plane
        float startX = center.x - totalWidth/2;
        float startZ = center.z - totalDepth/2;

        // First, create all cells
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                float x = startX + col * sizeOfCell + sizeOfCell / 2;
                float z = startZ + row * sizeOfCell + sizeOfCell / 2;
                Vector3 cellCenter = new Vector3(x, gridHeight, z);

                bool isObstacle = Random.Range(0f, 1f) <= 0.3f;

                GridCell cell = new GridCell(col + numColumns * row, cellCenter, isObstacle);
                nodes.Add(cell);

                if (isObstacle && obstaclePrefab != null)
                {
                    GameObject obstacle = GameObject.Instantiate(obstaclePrefab, cellCenter, Quaternion.identity);
                    obstacle.transform.localScale = new Vector3(sizeOfCell, sizeOfCell, sizeOfCell);
                }

                connections.Add(new GridConnections());
            }
        }

        // Then, create connections for each cell
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                int currentIndex = col + numColumns * row;
                GridCell currentCell = nodes[currentIndex];
                
                if (currentCell.IsOccupied)
                    continue;

                // Define all eight directions (including diagonals)
                var directions = new (int dx, int dy)[]
                {
                    (-1, -1), (0, -1), (1, -1),  // Top row
                    (-1,  0),          (1,  0),  // Middle row
                    (-1,  1), (0,  1), (1,  1)   // Bottom row
                };

                foreach (var dir in directions)
                {
                    int newRow = row + dir.dy;
                    int newCol = col + dir.dx;

                    // Check if the neighbor is within bounds
                    if (newRow >= 0 && newRow < numRows && newCol >= 0 && newCol < numColumns)
                    {
                        int neighborIndex = newCol + numColumns * newRow;
                        GridCell neighbor = nodes[neighborIndex];

                        if (!neighbor.IsOccupied)
                        {
                            float cost = (dir.dx != 0 && dir.dy != 0) ? 1.414f : 1f; // Diagonal cost vs straight cost
                            var connection = new CellConnection(currentCell, neighbor);
                            connection.setCost(cost);
                            connections[currentIndex].connections.Add(connection);
                        }
                    }
                }
            }
        }
    }
}
