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

    void Start()
    {
        targetPos = transform.position;

        // 👉 Đăng ký rail
        RailTile rail = GetComponent<RailTile>();
        if (rail != null)
        {
            rail.gridPos = gridPos;
            GridRailManager.Instance.Register(rail);
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
        GridRailManager.Instance.grid[gridPos.x, gridPos.y] = null;

        gridPos = newPos;
        targetPos = new Vector3(newPos.x, 0, newPos.y);

        // 👉 CẬP NHẬT rail mới
        if (rail != null)
        {
            rail.gridPos = newPos;
            GridRailManager.Instance.Register(rail);
        }
    }

    bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 4&&
               pos.y >= 0 && pos.y < 4;
    }

    bool IsEmpty(Vector2Int pos)
    {
        RailTile tile = GridRailManager.Instance.GetTile(pos);

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
