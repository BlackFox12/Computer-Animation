using UnityEngine;
using PathFinding;

public class GridCell : Node
{
    protected float xMin;
    protected float xMax;
    protected float zMin;
    protected float zMax;
    protected bool occupied;
    public Vector3 center;
    public float cellSize = 1;

    public GridCell(int i) : base(i)
    {
        xMin = 0f;
        xMax = 0f;
        zMin = 0f;
        zMax = 0f;
        occupied = false;
        center = Vector3.zero;
    }

    public GridCell(GridCell n) : base(n)
    {
        xMin = n.xMin;
        xMax = n.xMax;
        zMin = n.zMin;
        zMax = n.zMax;
        occupied = n.occupied;
        center = n.center;
    }

    public GridCell(int i, Vector3 center, bool isOccupied) : base(i)
    {
        this.center = center;
        xMin = center.x - cellSize / 2;
        xMax = center.x + cellSize / 2;
        zMin = center.z - cellSize / 2;
        zMax = center.z + cellSize / 2;
        occupied = isOccupied;
    }

    public void SetBounds(float xMin, float xMax, float zMin, float zMax)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.zMin = zMin;
        this.zMax = zMax;
    }

    public void SetOccupied(bool isOccupied)
    {
        this.occupied = isOccupied;
    }

    public bool IsOccupied => occupied;

    public override string ToString()
    {
        return $"GridCell {id} - xMin: {xMin}, xMax: {xMax}, zMin: {zMin}, zMax: {zMax}, center: {center}, Occupied: {occupied}";
    }
}
