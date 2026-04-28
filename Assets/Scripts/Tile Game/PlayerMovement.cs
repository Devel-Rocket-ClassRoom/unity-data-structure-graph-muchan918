using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;

    private int currentTileId;
    private int currentX;
    private int currentY;
    private int scope = 2;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        //animator.speed = 0f;

        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }

    private void Update()
    {
        var direction = Sides.None;

        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Sides.Top;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction = Sides.Bottom;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction = Sides.Right;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = Sides.Left;
        }

        if (direction != Sides.None)
        {
            var targetTile = stage.Map.tiles[currentTileId].adjacents[(int)direction];
            if (targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile.id);
            }
        }
    }

    public void MoveTo(int tileId)
    {
        currentTileId = tileId;
        currentX = tileId % stage.Map.cols;
        currentY = tileId / stage.Map.cols;

        transform.position = stage.GetTilePos(currentTileId);

        CheckVisit(tileId);
    }

    public void CheckVisit(int tileId)
    {
        for (int y = currentY - scope; y <= currentY + scope; y++)
        {
            for (int x = currentX - scope; x <= currentX + scope; x++)
            {
                if (x < 0 || x >= stage.Map.cols || y < 0 || y >= stage.Map.rows) continue;

                int id = y * stage.Map.cols + x;
                stage.Map.tiles[id].isVisited = true;
            }
        }

        for (int y = currentY - scope; y <= currentY + scope; y++)
        {
            for (int x = currentX - scope; x <= currentX + scope; x++)
            {
                if (x < 0 || x >= stage.Map.cols || y < 0 || y >= stage.Map.rows) continue;

                int id = y * stage.Map.cols + x;

                for (int i = 0; i < stage.Map.tiles[id].adjacents.Length; i++)
                {
                    if (stage.Map.tiles[id].adjacents[i] != null)
                    {
                        stage.DecorateTile(stage.Map.tiles[id].adjacents[i].id);
                    }
                }
            }
        }
    }

    public void MoveTo(int x, int y)
    {

    }
}
