using UnityEngine;
using System.Collections.Generic;
using PathFinding;

public class Grid : FiniteGraph<GridCell, CellConnection, GridConnections>
{
    protected float xMin;
    protected float xMax;
    protected float zMin;
    protected float zMax;
    protected float sizeOfCell;
    protected float gridHeight;
    protected int numRows;
    protected int numColumns;

    public GameObject obstaclePrefab; // Assign this in the Unity Editor

    public Grid(float minX, float maxX, float minZ, float maxZ, float cellSize, float height = 0, GameObject obstaclePrefab = null)
    {
        xMin = minX;
        xMax = maxX;
        zMin = minZ;
        zMax = maxZ;
        sizeOfCell = cellSize;
        gridHeight = height;

        numRows = Mathf.CeilToInt((zMax - zMin) / sizeOfCell);
        numColumns = Mathf.CeilToInt((xMax - xMin) / sizeOfCell);

        nodes = new List<GridCell>();
        connections = new List<GridConnections>();
        this.obstaclePrefab = obstaclePrefab;
    
        GameObject floorPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        float gridWidth = xMax - xMin;
        float gridDepth = zMax - zMin;

        
        Vector3 planeCenter = new Vector3(xMin + gridWidth / 2, -sizeOfCell, zMin + gridDepth / 2);
        floorPlane.transform.position = planeCenter;

        
        floorPlane.transform.localScale = new Vector3(gridWidth / 10f, 1, gridDepth / 10f);

        
        Renderer planeRenderer = floorPlane.GetComponent<Renderer>();
        planeRenderer.material.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Light gray

        GenerateGrid();
    }

    protected void GenerateGrid()
    {
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                float x = xMin + col * sizeOfCell + sizeOfCell / 2;
                float z = zMin + row * sizeOfCell + sizeOfCell / 2;
                Vector3 center = new Vector3(x, gridHeight, z);

                bool isObstacle = Random.Range(0f, 1f) <= 0.3f;

                GridCell cell = new GridCell(col + numColumns * row, center, isObstacle);
                nodes.Add(cell);

                if (isObstacle && obstaclePrefab != null)
                {
                    GameObject obstacle = GameObject.Instantiate(obstaclePrefab, center, Quaternion.identity);
                    obstacle.transform.localScale = new Vector3(sizeOfCell, sizeOfCell, sizeOfCell);
                }

                var cellConnections = new GridConnections();
                AddConnections(cell, row, col, cellConnections);
                connections.Add(cellConnections);
            }
        }
    }

    protected void AddConnections(GridCell cell, int row, int col, GridConnections cellConnections)
    {
        // Define directions for potential neighbors
        var directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0)  // Left
    };

        foreach (var dir in directions)
        {
            int neighborRow = row + dir.y;
            int neighborCol = col + dir.x;

            // Check if the neighbor is within grid bounds
            if (neighborRow >= 0 && neighborRow < numRows &&
                neighborCol >= 0 && neighborCol < numColumns)
            {
                int neighborIndex = neighborRow * numColumns + neighborCol;

                // Ensure the neighborIndex is valid
                if (neighborIndex >= 0 && neighborIndex < nodes.Count)
                {
                    GridCell neighbor = nodes[neighborIndex];

                    // Create connection if the neighbor is not an obstacle
                    if (!neighbor.IsOccupied)
                    {
                        var connection = new CellConnection(cell, neighbor);
                        connection.setCost(Vector3.Distance(cell.center, neighbor.center));
                        cellConnections.connections.Add(connection);
                    }
                }
            }
        }
    }
}
