using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    private readonly Vector2Int startCell;
    private readonly Vector2Int targetCell;
    private readonly bool[,] grid;

    // Nodes that we are interested in exploring
    private readonly HashSet<Node> openNodes = new HashSet<Node>();
    // Nodes that we are no longer interested in exploring
    private readonly HashSet<Node> closedNodes = new HashSet<Node>();

    private readonly Node[,] nodeGraph;

    // Pathfinder can only move in orthogonal directions
    private static readonly Vector2Int[] directions = { new(0, -1), new(0, 1), new(-1, 0), new(1, 0) };

    private readonly int wallCost; // Cost to path through a filled cell
    private readonly int emptyCost; // Cost to path through an empty cell
    private readonly int turnCost; // Cost to make a turn

    public Pathfinder(Vector2Int startCell, Vector2Int targetCell, bool[,] grid, int wallCost, int emptyCost, int turnCost)
    {
        this.startCell = startCell;
        this.targetCell = targetCell;
        this.grid = grid;

        this.wallCost = wallCost;
        this.emptyCost = emptyCost;
        this.turnCost = turnCost;

        // Initialize a node for each cell in the grid
        nodeGraph = new Node[grid.GetLength(0), grid.GetLength(1)];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                int distanceToTarget = (int)Vector2Int.Distance(new Vector2Int(x, y), targetCell);
                nodeGraph[x, y] = new Node(x, y, distanceToTarget);
            }
        }
    }

    // Get the list of cells that make up the path
    public List<Vector2Int> FindPath()
    {
        var path = new List<Vector2Int>();

        var startNode = nodeGraph[startCell.x, startCell.y];
        var targetNode = nodeGraph[targetCell.x, targetCell.y];
        openNodes.Add(startNode);

        // Repeat while there are still nodes to explore
        while (openNodes.Count != 0)
        {
            var currentNode = GetBestNode(openNodes);
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            // Found path
            if (currentNode == targetNode)
            {
                return currentNode.GetPath();
            }

            foreach (var dir in directions)
            {
                int neighborX = currentNode.x + dir.x;
                int neighborY = currentNode.y + dir.y;

                if (!InBounds(neighborX, neighborY)) continue;

                var neighbor = nodeGraph[neighborX, neighborY];

                // Skip closed nodes
                if (closedNodes.Contains(neighbor)) continue;

                int heuristicCost = GetHeuristicCost(currentNode, neighbor);

                // Find the new cost to reach this neighbor using the current path                
                int cost = currentNode.cost + heuristicCost;

                // If the neighbor has not been explored,
                // or if the current path to the neighbor is better than the neighbor's previous cost.
                // update the new best path to this neighbor
                if (!openNodes.Contains(neighbor) || cost < neighbor.cost)
                {
                    neighbor.cost = cost;
                    neighbor.parent = currentNode;
                    openNodes.Add(neighbor);
                }
            }
        }

        return path;
    }

    // Heuristic cost to move from current node to neighbor
    private int GetHeuristicCost(Node currentNode, Node neighbor)
    {
        int hCost = neighbor.distanceToTarget;

        hCost += grid[neighbor.x, neighbor.y] ? wallCost : emptyCost;

        // Check if the direction from the current node to this neighbor
        // is the same as the current path direction
        var dirToNeighbor = new Vector2Int(neighbor.x - currentNode.x, neighbor.y - currentNode.y);
        bool directionChanged = dirToNeighbor != currentNode.GetPathDirection();
        // If path direction was changed, add the cost of making a turn
        if (directionChanged)
        {
            hCost += turnCost;
        }

        return hCost;
    }

    private bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < nodeGraph.GetLength(0) && y < nodeGraph.GetLength(1);
    }

    private Node GetBestNode(HashSet<Node> nodes)
    {
        // The best node is the one with the lowest f cost
        return nodes.OrderBy(node => node.cost).FirstOrDefault();
    }

    class Node
    {
        public readonly int x;
        public readonly int y;
        public readonly int distanceToTarget;

        public int cost; // Lowest cost so far to this node from start node

        public Node parent; // Current parent of this node on the current path

        public Node(int x, int y, int distanceToTarget)
        {
            this.x = x;
            this.y = y;
            this.distanceToTarget = distanceToTarget;
        }

        // Get the direction to this node from its parent node
        // We consider this to be the current path direction
        public Vector2Int GetPathDirection()
        {
            if (parent == null) return Vector2Int.zero;
            return new Vector2Int(x - parent.x, y - parent.y);
        }

        public List<Vector2Int> GetPath()
        {
            return RecursiveGetPath(new List<Vector2Int>());
        }
        // Get the list of nodes from this node to the start node by recursively
        // finding parent nodes
        private List<Vector2Int> RecursiveGetPath(List<Vector2Int> path)
        {
            path.Add(new Vector2Int(x, y));
            if (parent == null)
            {
                return path;
            }
            return parent.RecursiveGetPath(path);
        }

    }
}