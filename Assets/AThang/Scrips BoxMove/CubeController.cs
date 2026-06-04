using UnityEngine;

public class CubeController : MonoBehaviour
{
    public enum CubeType
    {
        Normal,     // cube thường
        Fixed       // cube đứng im
    }

    public CubeType cubeType = CubeType.Normal;

    public Vector2Int gridPos;
    public float moveSpeed = 10f;

    Vector3 targetPos;
    public GridRailManager grid;

    void Start()
    {
        targetPos = grid.origin +
             new Vector3(gridPos.x, 0, gridPos.y);

        transform.position = targetPos;

        if (grid == null)
            grid = GetComponentInParent<GridRailManager>();

        RailTile rail = GetComponent<RailTile>();

        if (rail != null)
        {
            rail.gridPos = gridPos;
            grid.Register(rail);
        }
    }

    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * moveSpeed
        );
    }

    public void TryMove(Vector2Int dir)
    {
        // ❌ Cube đứng im thì không cho move
        if (cubeType == CubeType.Fixed)
            return;
        Vector2Int newPos = gridPos + dir;

        if (!IsInside(newPos)) return;
        if (!IsEmpty(newPos)) return;

        // 👉 XÓA rail cũ
        RailTile rail = GetComponent<RailTile>();
        grid.grid[gridPos.x, gridPos.y] = null;

        gridPos = newPos;
        targetPos = grid.origin +
             new Vector3(newPos.x, 0, newPos.y);

        MoveUI.Instance.AddMove();  // Tinh điểm Move 

        // 👉 CẬP NHẬT rail mới
        if (rail != null)
        {
            rail.gridPos = newPos;
            grid.Register(rail);
        }

    }

    bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < grid.width &&
               pos.y >= 0 && pos.y < grid.height;
    }

    bool IsEmpty(Vector2Int pos)
    {
        RailTile tile = grid.GetTile(pos);

        // ô trống hoàn toàn
        if (tile == null)
            return true;

        CubeController cube = tile.GetComponent<CubeController>();

        // ❌ có cube (Normal hay Fixed đều block)
        if (cube != null)
            return false;

        return true;
    }



}
