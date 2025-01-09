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

                // Instead of checking all neighbors, identify and process jump points
                foreach (var connection in graph.getConnections(current.node).connections)
                {
                    TNode jumpPoint = FindJumpPoint(graph, current.node, connection.getToNode(), end);
                    
                    if (jumpPoint == null || closedSet.Contains(jumpPoint))
                        continue;

                    float jumpPointCost = CalculateJumpPointCost(current.node, jumpPoint);
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

        private TNode FindJumpPoint(TGraph graph, TNode current, TNode direction, TNode goal)
        {
            GridCell currentCell = current as GridCell;
            GridCell directionCell = direction as GridCell;
            GridCell goalCell = goal as GridCell;
            
            if (currentCell == null || directionCell == null || goalCell == null)
                return null;

            // Calculate direction vector
            Vector3 dir = (directionCell.center - currentCell.center).normalized;
            int dx = Mathf.RoundToInt(dir.x);
            int dz = Mathf.RoundToInt(dir.z);

            // Current position in grid coordinates
            Vector3 currentPos = currentCell.center;
            
            while (true)
            {
                // Move to next position
                currentPos += new Vector3(dx * currentCell.cellSize, 0, dz * currentCell.cellSize);
                
                // Get the cell at the new position
                TNode nextNode = FindCellAtPosition(graph, currentPos);
                if (nextNode == null || (nextNode as GridCell).IsOccupied)
                    return null;

                // Check if we found the goal
                if (nextNode.Equals(goal))
                    return nextNode;

                // Check for forced neighbors
                if (HasForcedNeighbors(graph, nextNode as GridCell, dx, dz))
                    return nextNode;

                // If moving diagonally, check both cardinal directions
                if (dx != 0 && dz != 0)
                {
                    // Check horizontal jump point
                    TNode horizontalJump = FindJumpPoint(graph, nextNode, 
                        FindCellAtPosition(graph, currentPos + new Vector3(dx * currentCell.cellSize, 0, 0)), 
                        goal);
                    if (horizontalJump != null)
                        return nextNode;

                    // Check vertical jump point
                    TNode verticalJump = FindJumpPoint(graph, nextNode, 
                        FindCellAtPosition(graph, currentPos + new Vector3(0, 0, dz * currentCell.cellSize)), 
                        goal);
                    if (verticalJump != null)
                        return nextNode;
                }
            }
        }

        private TNode FindCellAtPosition(TGraph graph, Vector3 position)
        {
            // Start from any node and use connections to traverse
            foreach (var connection in graph.getConnections(allNodes.Keys.First()).connections)
            {
                GridCell cell = connection.getToNode() as GridCell;
                if (cell != null && 
                    position.x >= cell.center.x - cell.cellSize/2 && 
                    position.x <= cell.center.x + cell.cellSize/2 &&
                    position.z >= cell.center.z - cell.cellSize/2 && 
                    position.z <= cell.center.z + cell.cellSize/2)
                {
                    return connection.getToNode();
                }
            }
            return null;
        }

        private bool HasForcedNeighbors(TGraph graph, GridCell current, int dx, int dz)
        {
            if (current == null) return false;

            // For horizontal movement
            if (dz == 0 && dx != 0)
            {
                // Check cells above and below
                bool topBlocked = IsCellBlocked(graph, current.center + new Vector3(0, 0, current.cellSize));
                bool bottomBlocked = IsCellBlocked(graph, current.center + new Vector3(0, 0, -current.cellSize));
                
                return (topBlocked && !IsCellBlocked(graph, current.center + new Vector3(dx * current.cellSize, 0, current.cellSize))) ||
                       (bottomBlocked && !IsCellBlocked(graph, current.center + new Vector3(dx * current.cellSize, 0, -current.cellSize)));
            }
            
            // For vertical movement
            if (dx == 0 && dz != 0)
            {
                // Check cells left and right
                bool leftBlocked = IsCellBlocked(graph, current.center + new Vector3(-current.cellSize, 0, 0));
                bool rightBlocked = IsCellBlocked(graph, current.center + new Vector3(current.cellSize, 0, 0));
                
                return (leftBlocked && !IsCellBlocked(graph, current.center + new Vector3(-current.cellSize, 0, dz * current.cellSize))) ||
                       (rightBlocked && !IsCellBlocked(graph, current.center + new Vector3(current.cellSize, 0, dz * current.cellSize)));
            }
            
            // For diagonal movement
            if (dx != 0 && dz != 0)
            {
                return HasForcedNeighbors(graph, current, dx, 0) || HasForcedNeighbors(graph, current, 0, dz);
            }

            return false;
        }

        private bool IsCellBlocked(TGraph graph, Vector3 position)
        {
            TNode node = FindCellAtPosition(graph, position);
            return node == null || (node as GridCell).IsOccupied;
        }

        private float CalculateJumpPointCost(TNode from, TNode to)
        {
            GridCell fromCell = from as GridCell;
            GridCell toCell = to as GridCell;
            
            if (fromCell == null || toCell == null)
                return float.MaxValue;

            // Calculate the actual distance between the cells
            Vector3 diff = toCell.center - fromCell.center;
            float dx = Mathf.Abs(diff.x);
            float dz = Mathf.Abs(diff.z);

            // If moving diagonally, use diagonal distance formula
            if (dx > 0 && dz > 0)
            {
                float diagonal = Mathf.Min(dx, dz);
                float straight = Mathf.Abs(dx - dz);
                return diagonal * 1.414f + straight; // 1.414 is approximately sqrt(2)
            }
            
            // If moving straight, use Manhattan distance
            return dx + dz;
        }

        protected List<TNode> BuildPath(NodeRecord endRecord)
        {
            List<TNode> path = new List<TNode>();
            NodeRecord current = endRecord;

            while (current != null)
            {
                path.Add(current.node);
                current = current.parent;
            }

            path.Reverse();
            return path;
        }
    }
} 