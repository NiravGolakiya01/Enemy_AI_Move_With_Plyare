using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static List<Vector3> FindPath(Vector3 startPosition, Vector3 targetPosition, bool[] obstacles, int gridSize)
    {
        Node startNode = new Node(startPosition);
        Node targetNode = new Node(targetPosition);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.Equals(targetNode))
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Node neighbour in GetNeighbours(currentNode, gridSize, obstacles))
            {
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    private static List<Node> GetNeighbours(Node node, int gridSize, bool[] obstacles)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSize && checkY >= 0 && checkY < gridSize)
                {
                    int index = checkY * gridSize + checkX;
                    if (!obstacles[index])
                    {
                        neighbours.Add(new Node(new Vector3(checkX, 0, checkY), node));
                    }
                }
            }
        }

        return neighbours;
    }

    private static List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private class Node
    {
        public int gridX;
        public int gridY;
        public Vector3 worldPosition;
        public int gCost;
        public int hCost;
        public Node parent;

        public Node(Vector3 worldPosition, Node parent = null)
        {
            this.worldPosition = worldPosition;
            this.parent = parent;
            this.gridX = Mathf.RoundToInt(worldPosition.x);
            this.gridY = Mathf.RoundToInt(worldPosition.z);
        }

        public int fCost
        {
            get { return gCost + hCost; }
        }

        public bool Equals(Node other)
        {
            return other != null && gridX == other.gridX && gridY == other.gridY;
        }
    }
}
