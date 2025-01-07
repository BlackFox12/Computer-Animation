using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PathFinding;

public class GridHeuristic : Heuristic<GridCell>
{
    // Class that represents a Heuristic function to estimate the cost of going from 
    // one GridCell to another

    // constructor takes a goal node for estimating
    public GridHeuristic(GridCell goal) : base(goal)
    {
        goalNode = goal;
    }

    // generates an estimated cost to reach the stored goal from the given node
    public override float estimateCost(GridCell fromNode)
    {
        // Using Euclidean distance as the heuristic
        Vector3 fromPos = fromNode.center;
        Vector3 goalPos = goalNode.center;
        return Vector3.Distance(fromPos, goalPos);
    }

    // determines if the goal node has been reached by node
    public override bool goalReached(GridCell node)
    {
        // Check if the current node is the goal node
        return node == goalNode;
    }
};
