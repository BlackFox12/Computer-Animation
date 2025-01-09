using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding
{
    public class A_Star<TNode, TConnection, TNodeConnection, TGraph, THeuristic> : PathFinder<TNode, TConnection, TNodeConnection, TGraph, THeuristic>
        where TNode : Node
        where TConnection : Connection<TNode>
        where TNodeConnection : NodeConnections<TNode, TConnection>
        where TGraph : Graph<TNode, TConnection, TNodeConnection>
        where THeuristic : Heuristic<TNode>
    {
        protected List<TNode> visitedNodes;
        protected Dictionary<TNode, NodeRecord> allNodes;
        protected SortedSet<NodeRecord> openSet;
        protected HashSet<TNode> closedSet;

        public A_Star(int maxNodes, float maxTime, int maxDepth) : base()
        {
            visitedNodes = new List<TNode>();
            allNodes = new Dictionary<TNode, NodeRecord>();
            openSet = new SortedSet<NodeRecord>(new NodeRecordComparer());
            closedSet = new HashSet<TNode>();
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
                // Get the node with lowest fCost
                NodeRecord current = openSet.Min;
                openSet.Remove(current);

                if (current.node.Equals(end))
                {
                    found = 1;
                    return BuildPath(current);
                }

                closedSet.Add(current.node);

                // Check all neighbors
                foreach (var connection in graph.getConnections(current.node).connections)
                {
                    TNode neighbor = connection.getToNode();
                    
                    if (closedSet.Contains(neighbor))
                        continue;

                    float tentativeGCost = current.gCost + connection.getCost();

                    if (!allNodes.ContainsKey(neighbor))
                    {
                        // New node discovered
                        NodeRecord neighborRecord = new NodeRecord
                        {
                            node = neighbor,
                            parent = current,
                            gCost = tentativeGCost,
                            hCost = heuristic.estimateCost(neighbor)
                        };

                        allNodes[neighbor] = neighborRecord;
                        openSet.Add(neighborRecord);
                        visitedNodes.Add(neighbor);
                    }
                    else
                    {
                        NodeRecord neighborRecord = allNodes[neighbor];
                        if (tentativeGCost < neighborRecord.gCost)
                        {
                            // Found a better path
                            openSet.Remove(neighborRecord);
                            neighborRecord.gCost = tentativeGCost;
                            neighborRecord.parent = current;
                            openSet.Add(neighborRecord);
                        }
                    }
                }
            }

            found = -1;
            return new List<TNode>();
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

        public virtual List<TNode> getVisitedNodes()
        {
            return visitedNodes;
        }

        public bool IsInOpenSet(TNode node)
        {
            return allNodes.ContainsKey(node) && openSet.Contains(allNodes[node]);
        }

        public bool IsInClosedSet(TNode node)
        {
            return closedSet.Contains(node);
        }

        protected class NodeRecord
        {
            public TNode node;
            public NodeRecord parent;
            public float gCost;  // Cost from start to current
            public float hCost;  // Estimated cost from current to end
            public float fCost => gCost + hCost;  // Total estimated cost
        }

        protected class NodeRecordComparer : IComparer<NodeRecord>
        {
            public int Compare(NodeRecord x, NodeRecord y)
            {
                int comparison = x.fCost.CompareTo(y.fCost);
                if (comparison == 0)
                {
                    // If fCosts are equal, prefer the one with lower hCost
                    comparison = x.hCost.CompareTo(y.hCost);
                }
                if (comparison == 0)
                {
                    // If still equal, use node IDs to ensure consistent ordering
                    comparison = x.node.getId().CompareTo(y.node.getId());
                }
                return comparison;
            }
        }
    }
}
