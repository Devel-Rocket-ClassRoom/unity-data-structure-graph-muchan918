using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Graph
{
    public int row = 0;
    public int col = 0;

    public GraphNode[] nodes;

    public void Init(int[,] grid)
    {
        row = grid.GetLength(0);
        col = grid.GetLength(1);

        nodes = new GraphNode[grid.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new GraphNode();
            nodes[i].id = i;
        }

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {
                int index = r * col + c;
                nodes[index].weight = grid[r, c];

                if (grid[r, c] == -1)
                    continue;

                if (r - 1 >= 0 && grid[r - 1, c] >= 0)
                {
                    nodes[index].adjacents.Add(nodes[index - col]); // up
                }
                if (c + 1 < col && grid[r, c + 1] >= 0)
                {
                    nodes[index].adjacents.Add(nodes[index + 1]); // right
                }
                if (r + 1 < row && grid[r + 1, c] >= 0)
                {
                    nodes[index].adjacents.Add(nodes[index + col]); // down
                }
                if (c - 1 >= 0 && grid[r, c - 1] >= 0)
                {
                    nodes[index].adjacents.Add(nodes[index - 1]); // left
                }
            }
        }
    }

    public void ResetNodePrevious()
    {
        foreach (var node in nodes)
        {
            node.previous = null;
        }
    }
}
