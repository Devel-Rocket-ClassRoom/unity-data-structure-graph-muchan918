using System.Collections.Generic;
using UnityEngine;

public class TileSearch
{
    private Map map;
    private HashSet<Tile> visited;
    public List<Tile> path = new List<Tile>();

    public void Init(Map map)
    {
        this.map = map;
        visited = new HashSet<Tile>();
    }

    public bool AStar(Tile start, Tile end)
    {
        path.Clear();
        visited.Clear();
        map.ResetTilePrevious();

        var dist = new Dictionary<Tile, int>();
        var priorityQueue = new PriorityQueue<Tile, int>();
        var currentTile = start;

        foreach (var tile in map.tiles)
        {
            dist[tile] = int.MaxValue;
        }

        priorityQueue.Enqueue(start, 0);
        dist[start] = 0;

        while (priorityQueue.Count > 0 && currentTile != end)
        {
            currentTile = priorityQueue.Dequeue();
            visited.Add(currentTile);

            foreach (var adjacent in currentTile.adjacents)
            {
                if (adjacent == null || !adjacent.CanMove || visited.Contains(adjacent)) continue;

                var newDist = dist[currentTile] + adjacent.weight;

                if (newDist < dist[adjacent])
                {
                    dist[adjacent] = newDist;
                    adjacent.previous = currentTile;
                    // 여기서 휴리스틱 추가
                    priorityQueue.Enqueue(adjacent, newDist + Heuristic(adjacent, end));
                }
            }
        }

        if (dist[end] == int.MaxValue) return false;

        while (currentTile != null)
        {
            path.Add(currentTile);
            currentTile = currentTile.previous;
        }
        path.Reverse();

        return true;
    }

    private int Heuristic(Tile a, Tile b)
    {
        int ax = a.id % map.cols;
        int ay = a.id / map.cols;

        int bx = b.id % map.cols;
        int by = b.id / map.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }
}
