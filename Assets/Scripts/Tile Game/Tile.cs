using UnityEngine;

// 0000
// 0001
// 0010
// 0011

// 1000

public enum Sides
{
    Bottom, // 3
    Right,  // 2
    Left,   // 1
    Top,    // 0
}

public class Tile
{
    public int id;
    public Tile[] adjacents = new Tile[4];

    public int autoTileId;

    // 맵이 열려있는지 판단
    public bool isVisited = false;

    public void UpdateAutoTileId()
    {
        autoTileId = 0;

        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                autoTileId |= 1 << adjacents.Length - 1 - i;
            }
        }
    }
}
