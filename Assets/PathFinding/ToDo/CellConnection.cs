using UnityEngine;
using PathFinding;

public class CellConnection : Connection<GridCell>
{
    public CellConnection(GridCell from, GridCell to) : base(from, to)
    {
        setCost(Vector3.Distance(from.center, to.center));
    }
}
