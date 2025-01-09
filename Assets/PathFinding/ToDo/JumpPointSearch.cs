using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding
{
    public class JumpPointSearch<TNode, TConnection, TNodeConnection, TGraph, THeuristic>
        : A_Star<TNode, TConnection, TNodeConnection, TGraph, THeuristic>
        where TNode : Node
        where TConnection : Connection<TNode>
        where TNodeConnection : NodeConnections<TNode, TConnection>
        where TGraph : Graph<TNode, TConnection, TNodeConnection>
        where THeuristic : Heuristic<TNode>
    {
        public JumpPointSearch(int maxNodes, float maxTime, int maxDepth) : base(maxNodes, maxTime, maxDepth)
        {
        }

        public override List<TNode> findpath(TGraph graph, TNode start, TNode end, THeuristic heuristic, ref int found)
        {
            Debug.Log($"Starting JPS pathfinding from {(start as GridCell).center} to {(end as GridCell).center}");
            // Clear previous search data
            visitedNodes.Clear();
            allNodes.Clear();
            openSet.Clear();
            closedSet.Clear();

            // Initialize the start node
            NodeRecord startRecord = new NodeRecord
            {
                node = start,
                parent = null,
                gCost = 0,
                hCost = heuristic.estimateCost(start)
            };

            allNodes[start] = startRecord;
            openSet.Add(startRecord);
            visitedNodes.Add(start);

            while (openSet.Count > 0)
            {
                NodeRecord current = openSet.Min;
                openSet.Remove(current);

                if (current.node.Equals(end))
                {
                    found = 1;
                    return BuildPath(current);
                }

                closedSet.Add(current.node);

                foreach (var connection in graph.getConnections(current.node).connections)
                {
                    TNode jumpPoint = IdentifyJumpPoint(graph, current.node, connection.getToNode(), end);

                    if (jumpPoint == null || closedSet.Contains(jumpPoint))
                        continue;

                    float jumpPointCost = heuristic.estimateCost(current.node, jumpPoint);
                    float tentativeGCost = current.gCost + jumpPointCost;

                    if (!allNodes.ContainsKey(jumpPoint))
                    {
                        NodeRecord jumpPointRecord = new NodeRecord
                        {
                            node = jumpPoint,
                            parent = current,
                            gCost = tentativeGCost,
                            hCost = heuristic.estimateCost(jumpPoint)
                        };

                        allNodes[jumpPoint] = jumpPointRecord;
                        openSet.Add(jumpPointRecord);
                        visitedNodes.Add(jumpPoint);
                    }
                    else
                    {
                        NodeRecord jumpPointRecord = allNodes[jumpPoint];
                        if (tentativeGCost < jumpPointRecord.gCost)
                        {
                            openSet.Remove(jumpPointRecord);
                            jumpPointRecord.gCost = tentativeGCost;
                            jumpPointRecord.parent = current;
                            openSet.Add(jumpPointRecord);
                        }
                    }
                }
            }

            found = -1;
            return new List<TNode>();
        }

        private TNode IdentifyJumpPoint(TGraph graph, TNode current, TNode direction, TNode goal)
        {
            GridCell currentCell = current as GridCell;
            GridCell directionCell = direction as GridCell;
            GridCell goalCell = goal as GridCell;

            if (currentCell == null || directionCell == null || goalCell == null)
                return null;

            Vector3 dir = (directionCell.center - currentCell.center).normalized;
            int dx = Mathf.RoundToInt(dir.x);
            int dz = Mathf.RoundToInt(dir.z);
            Vector3 currentPos = currentCell.center;

            // Add maximum steps to prevent infinite loops
            int maxSteps = 100;  // Adjust based on your grid size
            int steps = 0;

            while (steps < maxSteps)
            {
                steps++;
                currentPos += new Vector3(dx * currentCell.cellSize, 0, dz * currentCell.cellSize);

                TNode nextNode = FindCellAtPosition(graph, currentPos);
                if (nextNode == null || (nextNode as GridCell).IsOccupied)
                    return null;

                // Add the node to visited nodes for visualization
                if (!visitedNodes.Contains(nextNode))
                    visitedNodes.Add(nextNode);

                if (nextNode.Equals(goal))
                    return nextNode;

                if (HasForcedNeighbors(graph, nextNode as GridCell, dx, dz))
                    return nextNode;

                // Diagonal movement checks
                if (dx != 0 && dz != 0)
                {
                    // Check horizontal and vertical directions
                    TNode horizontalJump = IdentifyJumpPoint(graph, nextNode,
                        FindCellAtPosition(graph, currentPos + new Vector3(dx * currentCell.cellSize, 0, 0)), goal);
                    
                    TNode verticalJump = IdentifyJumpPoint(graph, nextNode,
                        FindCellAtPosition(graph, currentPos + new Vector3(0, 0, dz * currentCell.cellSize)), goal);

                    if (horizontalJump != null || verticalJump != null)
                        return nextNode;
                }
            }

            return null;
        }

        private bool HasForcedNeighbors(TGraph graph, GridCell current, int dx, int dz)
        {
            if (current == null) return false;

            // Reduce the required number of clear neighbors
            int requiredClearNeighbors = 2;  // Changed from 3
            int clearNeighbors = 0;

            // Check all eight directions
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && z == 0) continue;

                    Vector3 neighborPos = current.center + new Vector3(x * current.cellSize, 0, z * current.cellSize);
                    if (!IsCellBlocked(graph, neighborPos))
                    {
                        clearNeighbors++;
                    }
                }
            }

            return clearNeighbors >= requiredClearNeighbors;
        }

        private bool IsCellBlocked(TGraph graph, Vector3 position)
        {
            TNode node = FindCellAtPosition(graph, position);
            return node == null || (node as GridCell).IsOccupied;
        }

        private TNode FindCellAtPosition(TGraph graph, Vector3 position)
        {
            foreach (var node in (graph as Grid).nodes)
            {
                GridCell cell = node as GridCell;
                if (cell != null &&
                    Mathf.Abs(position.x - cell.center.x) <= cell.cellSize / 2 &&
                    Mathf.Abs(position.z - cell.center.z) <= cell.cellSize / 2)
                {
                    return node as TNode;
                }
            }
            return null;
        }
    }
}