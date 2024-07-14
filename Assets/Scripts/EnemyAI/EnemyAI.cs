using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour, AI
{
    public float moveSpeed = 5f;
    private bool isMoving = false;

    public void MoveTowardsPlayer(Vector3 playerPosition)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveToAdjacentTile(playerPosition));
        }
    }

    private IEnumerator MoveToAdjacentTile(Vector3 playerPosition)
    {
        isMoving = true;

        List<Vector3> path = FindPathToPlayer(playerPosition);
        if (path != null && path.Count > 0)
        {
            foreach (Vector3 position in path)
            {
                while (Vector3.Distance(transform.position, position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
        }

        isMoving = false;
    }

    private List<Vector3> FindPathToPlayer(Vector3 playerPosition)
    {
        Vector2Int startGrid = new Vector2Int(Mathf.RoundToInt(transform.position.x / 1.1f), Mathf.RoundToInt(transform.position.z / 1.1f));
        Vector2Int playerGrid = new Vector2Int(Mathf.RoundToInt(playerPosition.x / 1.1f), Mathf.RoundToInt(playerPosition.z / 1.1f));

        List<Vector2Int> adjacentTiles = new List<Vector2Int>
        {
            new Vector2Int(playerGrid.x + 1, playerGrid.y),
            new Vector2Int(playerGrid.x - 1, playerGrid.y),
            new Vector2Int(playerGrid.x, playerGrid.y + 1),
            new Vector2Int(playerGrid.x, playerGrid.y - 1)
        };

        foreach (var tile in adjacentTiles)
        {
            if (!IsObstacle(tile))
            {
                List<Vector3> path = AStar(startGrid, tile);
                if (path != null)
                {
                    return path;
                }
            }
        }

        return null;
    }

    private bool IsObstacle(Vector2Int position)
    {
        ObstacleData obstacleData = FindObjectOfType<ObstacleManager>().obstacleData;
        return obstacleData.obstacles[position.y * 10 + position.x];
    }

    private List<Vector3> AStar(Vector2Int start, Vector2Int goal)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        Node startNode = new Node(start, null, 0, Heuristic(start, goal));
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.position == goal)
            {
                return RetracePath(start, currentNode);
            }

            foreach (Vector2Int neighbour in GetNeighbours(currentNode.position))
            {
                if (IsObstacle(neighbour) || closedSet.Contains(new Node(neighbour))) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode.position, neighbour);
                Node neighbourNode = new Node(neighbour, currentNode, newMovementCostToNeighbour, Heuristic(neighbour, goal));

                if (newMovementCostToNeighbour < neighbourNode.gCost || !openSet.Contains(neighbourNode))
                {
                    neighbourNode.gCost = newMovementCostToNeighbour;
                    neighbourNode.hCost = Heuristic(neighbour, goal);
                    neighbourNode.parent = currentNode;

                    if (!openSet.Contains(neighbourNode))
                    {
                        openSet.Add(neighbourNode);
                    }
                }
            }
        }

        return null;
    }

    private List<Vector2Int> GetNeighbours(Vector2Int nodePosition)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = nodePosition.x + x;
                int checkY = nodePosition.y + y;

                if (checkX >= 0 && checkX < 10 && checkY >= 0 && checkY < 10)
                {
                    neighbours.Add(new Vector2Int(checkX, checkY));
                }
            }
        }

        return neighbours;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private int GetDistance(Vector2Int a, Vector2Int b)
    {
        int dstX = Mathf.Abs(a.x - b.x);
        int dstY = Mathf.Abs(a.y - b.y);

        return dstX + dstY;
    }

    private List<Vector3> RetracePath(Vector2Int start, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != null && currentNode.position != start)
        {
            path.Add(new Vector3(currentNode.position.x * 1.1f, 0.5f, currentNode.position.y * 1.1f));
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private class Node
    {
        public Vector2Int position;
        public Node parent;
        public int gCost;
        public int hCost;

        public Node(Vector2Int position, Node parent = null, int gCost = 0, int hCost = 0)
        {
            this.position = position;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }

        public int FCost
        {
            get { return gCost + hCost; }
        }

        public override bool Equals(object obj)
        {
            return obj is Node node && position.Equals(node.position);
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
