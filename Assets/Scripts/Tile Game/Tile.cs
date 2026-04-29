using UnityEngine;

// 0000
// 0001
// 0010
// 0011
// 1000

public enum Sides
{
    None = -1,
    Top, // 3
    Left,  // 2
    Right,   // 1
    Bottom,    // 0
}

public class Tile
{
    public int id;
    public int weight = (int)TileWeight.Grass;
    public Tile previous = null;

    public Tile[] adjacents = new Tile[4];

    public int autoTileId;
    public int autoFowTileId;

    // 맵이 열려있는지 판단
    public bool isVisited = false;

    public bool CanMove => autoTileId != (int)TileTypes.Empty;

    public void UpdateAutoFowTileId()
    {
        autoFowTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null || !adjacents[i].isVisited)
            {
                autoFowTileId |= 1 << i;
            }
        }
    }

    public void UpdateAutoTileId()
    {
        autoTileId = 0;

        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                autoTileId |= 1 << i;
            }
        }
    }

    public void RemoveAdjacents(Tile tile)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null) continue;

            if (adjacents[i].id == tile.id)
            {
                adjacents[i] = null;
                UpdateAutoTileId();
                UpdateAutoFowTileId();
                break;
            }
        }
    }

    public void ClearAdjacents()
    {
        autoTileId = (int)TileTypes.Empty;

        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null) continue;

            adjacents[i].RemoveAdjacents(this);
            adjacents[i] = null;
            //adjacents[i].UpdateAutoTileId();
        }

        UpdateAutoTileId();
        UpdateAutoFowTileId();
    }
}
