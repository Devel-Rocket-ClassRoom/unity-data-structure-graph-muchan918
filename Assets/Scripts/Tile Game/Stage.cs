using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefabs;
    private GameObject[] tileObjs;

    public PlayerMovement playerPrefab;
    private PlayerMovement player;

    public int mapWidth = 20;
    public int mapHeight = 20;

    private float startX;
    private float startY;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIterations = 2;
    public float lakePercent = 0.01f;
    public float treePercent = 0.2f;
    public float hillPercent = 0.2f;
    public float mountainPercent = 0.1f;
    public float townPercent = 0.1f;
    public float monsterPercent = 0.1f;

    public Vector2 tileSize = new Vector2(16, 16);

    public Sprite[] islandSprites;
    public Sprite[] isFowSprites;

    private Map map;
    public Map Map => map;

    private int lastHoveredTileId = -1;

    private void Start()
    {
        startX = transform.position.x - mapWidth * tileSize.x * 0.5f + tileSize.x * 0.5f;
        startY = transform.position.y + mapHeight * tileSize.y * 0.5f - tileSize.y * 0.5f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
        }

        if (tileObjs == null) return;

        if (lastHoveredTileId != -1)
        {
            tileObjs[lastHoveredTileId].GetComponent<SpriteRenderer>().color = Color.white;
        }

        int tileId = ScreenPosToTileId(Input.mousePosition);
        if (tileId != -1)
        {
            tileObjs[tileId].GetComponent<SpriteRenderer>().color = Color.red;
            lastHoveredTileId = tileId;
        }
    }

    private void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        //map.CreateIsland(coastErodePercent, coastErodeIterations);
        map.CreateIsland(erodePercent, erodeIterations, lakePercent,
        treePercent, hillPercent, mountainPercent, townPercent, monsterPercent);
        CreateGrid();
        CreatePlayer(); // 여기서 오픈해주는걸 필요함
    }

    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }

        player = Instantiate(playerPrefab);
        player.InitPos(map.startTile.id);
    }

    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }

        tileObjs = new GameObject[mapWidth * mapHeight];

        var position = new Vector3(startX, startY, 0f);

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                var tileId = i * mapWidth + j;
                var newGo = Instantiate(tilePrefabs, transform);
                newGo.transform.position = position;
                position.x += tileSize.x;

                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            position.x = startX;
            position.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId]; // 데이터
        var tileGo = tileObjs[tileId]; // 게임 오브젝트
        var ren = tileGo.GetComponent<SpriteRenderer>();

        tile.UpdateAutoFowTileId();

        // 여기서 분기해서 fow 설정
        if (tile.isVisited)
        {
            if (tile.autoTileId != (int)TileTypes.Empty)
            {
                ren.sprite = islandSprites[tile.autoTileId];
            }
            else
            {
                ren.sprite = null;
            }
        }
        else
        {
            ren.sprite = isFowSprites[tile.autoFowTileId];
        }
    }

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return WorldPosToTileId(worldPos);
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        int col = Mathf.FloorToInt((worldPos.x - startX) / tileSize.x + 0.5f);
        int row = Mathf.FloorToInt((startY - worldPos.y) / tileSize.y + 0.5f);

        if (col < 0 || col >= mapWidth || row < 0 || row >= mapHeight)
            return -1;

        return row * mapWidth + col;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        return new Vector3(startX + x * tileSize.x, startY - y * tileSize.y, 0f);
    }

    public Vector3 GetTilePos(int tileId)
    {
        int row = tileId / mapWidth;
        int col = tileId % mapWidth;
        return GetTilePos(row, col);
    }
}
