using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject obstaclePrefab; // Assign in Inspector
    public float xMin = 0;
    public float xMax = 10;
    public float zMin = 0;
    public float zMax = 10;
    public float cellSize = 1;

    private Grid grid;

    void Start()
    {
        grid = new Grid(xMin, xMax, zMin, zMax, cellSize, 0, obstaclePrefab);
    }
}
