using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;

    private int currentTileId;
    public int CurrentTileId => currentTileId;
    //private int scope = 2;

    private bool isMoving = false;
    private float moveSpeed = 2f;
    private Coroutine coMove = null;
    private int pathWeight;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0f;
        Debug.Log(animator.speed);
        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }

    private void Update()
    {
        if (isMoving) return; // 이동 중엔 입력 무시

        var direction = Sides.None;

        if (Input.GetKeyDown(KeyCode.W)) direction = Sides.Top;
        else if (Input.GetKeyDown(KeyCode.S)) direction = Sides.Bottom;
        else if (Input.GetKeyDown(KeyCode.D)) direction = Sides.Right;
        else if (Input.GetKeyDown(KeyCode.A)) direction = Sides.Left;

        if (direction != Sides.None)
        {
            var targetTile = stage.Map.tiles[currentTileId].adjacents[(int)direction];
            if (targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile.id);
            }
        }
    }

    public void InitPos(int tileId)
    {
        currentTileId = tileId;

        transform.position = stage.GetTilePos(currentTileId);

        stage.OnTileVisited(tileId);
        //CheckVisit(tileId);
    }

    public void MoveToTargetTile(TileSearch search, int targetTileId)
    {
        if (isMoving) return;

        stage.ChangePathColor(search.path, Color.white);
        pathWeight = 0;

        if (search.AStar(stage.Map.tiles[currentTileId], stage.Map.tiles[targetTileId]))
        {
            if (coMove != null)
            {
                StopCoroutine(coMove);
                coMove = null;
            }

            coMove = StartCoroutine(MovePathCoroutine(search.path));
        }
        else
        {
            Debug.Log("갈 수 없음");
        }
    }

    private IEnumerator MovePathCoroutine(List<Tile> path)
    {
        for (int i = 1; i < path.Count; i++)
        {
            pathWeight += path[i].weight;
            yield return MoveCoroutine(path[i].id);
        }
        stage.ChangePathColor(path, Color.blue);
        Debug.Log($"경로 가중치 합: {pathWeight}");
    }

    public void MoveTo(int tileId)
    {
        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }

        coMove = StartCoroutine(MoveCoroutine(tileId)); // Lerp 이동 시작
        //CheckVisit(tileId);
    }

    private IEnumerator MoveCoroutine(int tileId)
    {
        isMoving = true;
        animator.speed = 2f;
        Vector3 startPos = transform.position;
        Vector3 endPos = stage.GetTilePos(tileId);
        float elapsed = 0f;
        float duration = 1f / moveSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        transform.position = endPos; // 정확한 위치로 보정
        currentTileId = tileId;
        isMoving = false;
        coMove = null;
        animator.speed = 0f;
        stage.OnTileVisited(tileId);
    }

    // public void CheckVisit(int tileId)
    // {
    //     for (int y = currentY - scope; y <= currentY + scope; y++)
    //     {
    //         for (int x = currentX - scope; x <= currentX + scope; x++)
    //         {
    //             if (x < 0 || x >= stage.Map.cols || y < 0 || y >= stage.Map.rows) continue;

    //             int id = y * stage.Map.cols + x;
    //             stage.Map.tiles[id].isVisited = true;
    //         }
    //     }

    //     for (int y = currentY - scope; y <= currentY + scope; y++)
    //     {
    //         for (int x = currentX - scope; x <= currentX + scope; x++)
    //         {
    //             if (x < 0 || x >= stage.Map.cols || y < 0 || y >= stage.Map.rows) continue;

    //             int id = y * stage.Map.cols + x;

    //             for (int i = 0; i < stage.Map.tiles[id].adjacents.Length; i++)
    //             {
    //                 if (stage.Map.tiles[id].adjacents[i] != null)
    //                 {
    //                     stage.DecorateTile(stage.Map.tiles[id].adjacents[i].id);
    //                 }
    //             }
    //         }
    //     }
    // }

    public void MoveTo(int x, int y)
    {
        MoveTo(x * stage.mapWidth + y);
    }
}
