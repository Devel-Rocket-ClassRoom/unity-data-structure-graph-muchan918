using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GraphSearch
{
    private Graph graph;
    private HashSet<GraphNode> visited;
    public List<GraphNode> path = new List<GraphNode>();

    public void Init(Graph graph)
    {
        this.graph = graph;
        visited = new HashSet<GraphNode>();
    }

    public void DFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var stack = new Stack<GraphNode>();

        stack.Push(node);
        visited.Add(node);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            path.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent)) continue;

                visited.Add(adjacent);
                stack.Push(adjacent);
            }
        }
    }

    public void BFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(node);
        visited.Add(node);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            path.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent)) continue;

                visited.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }
    }

    public void DFSRecursive(GraphNode node)
    {
        path.Clear();
        visited.Clear();
        DFSRecursiveInternal(node);
    }

    private void DFSRecursiveInternal(GraphNode node)
    {
        visited.Add(node);
        path.Add(node);

        foreach (var adjacent in node.adjacents)
        {
            if (!adjacent.CanVisit || visited.Contains(adjacent)) continue;

            DFSRecursiveInternal(adjacent);
        }
    }

    public bool PathFindingBFS(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        var currentNode = start;
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0 && currentNode.id != end.id)
        {
            currentNode = queue.Dequeue();

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent)) continue;
                adjacent.previous = currentNode;
                visited.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }

        if (currentNode.id != end.id) return false;

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.previous;
        }

        path.Reverse();

        return true;
    }

    public bool Dijkstra(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var dist = new Dictionary<GraphNode, int>();
        var priorityQueue = new PriorityQueue<GraphNode, int>();
        var currentNode = start;

        foreach (var node in graph.nodes)
        {
            dist[node] = int.MaxValue;
        }

        priorityQueue.Enqueue(start, 0);
        dist[start] = 0;

        while (priorityQueue.Count > 0 && currentNode != end)
        {
            currentNode = priorityQueue.Dequeue();

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit) continue;

                var newDist = dist[currentNode] + adjacent.weight;

                if (newDist < dist[adjacent])
                {
                    dist[adjacent] = newDist;
                    adjacent.previous = currentNode;
                    priorityQueue.Enqueue(adjacent, newDist);
                }
            }
        }

        if (dist[end] == int.MaxValue) return false;

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.previous;
        }
        path.Reverse();

        return true;
    }

    public bool AStar(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var dist = new Dictionary<GraphNode, int>();
        var priorityQueue = new PriorityQueue<GraphNode, int>();
        var currentNode = start;

        foreach (var node in graph.nodes)
        {
            dist[node] = int.MaxValue;
        }

        priorityQueue.Enqueue(start, 0);
        dist[start] = 0;

        while (priorityQueue.Count > 0 && currentNode != end)
        {
            currentNode = priorityQueue.Dequeue();

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit) continue;

                var newDist = dist[currentNode] + adjacent.weight;

                if (newDist < dist[adjacent])
                {
                    dist[adjacent] = newDist;
                    adjacent.previous = currentNode;
                    priorityQueue.Enqueue(adjacent, newDist + Heuristic(adjacent, end));
                }
            }
        }

        if (dist[end] == int.MaxValue) return false;

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.previous;
        }
        path.Reverse();

        return true;
    }

    private int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.col;
        int ay = a.id / graph.col;

        int bx = b.id % graph.col;
        int by = b.id / graph.col;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }
}
