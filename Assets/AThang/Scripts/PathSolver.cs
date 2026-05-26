using System.Collections.Generic;
using UnityEngine;

public class PathSolver : MonoBehaviour
{
    public RailTile startTile;
    public RailTile finishTile;

    public List<Vector3> finalPath = new List<Vector3>();

    public bool Solve()
    {

        finalPath.Clear();

        RailTile current = startTile;
        RailDirection dir = startTile.exit;

        finalPath.AddRange(startTile.GetWorldPath());

        int safety = 0;

        while (current != finishTile)
        {
            safety++;
            if (safety > 50) return false; // tránh loop vô hạn

            Vector2Int nextPos = current.gridPos + DirToVector(dir);
            RailTile next = GridRailManager.Instance.GetTile(nextPos);

            if (next == null)
            {
                Debug.Log("❌ Không có rail tiếp theo");
                return false;
            }

            if (next.entry != Opposite(dir))
            {
                Debug.Log("❌ Rail không khớp hướng");
                return false;
            }

            finalPath.AddRange(next.GetWorldPath());

            dir = next.exit;
            current = next;
            Debug.Log(
    $"Đang ở {current.name} | grid {current.gridPos} | exit {dir}"
);

        }

        Debug.Log("✅ Path hợp lệ");
        return true;

    }

    Vector2Int DirToVector(RailDirection dir)
    {
        return dir switch
        {
            RailDirection.Up => Vector2Int.up,
            RailDirection.Down => Vector2Int.down,
            RailDirection.Left => Vector2Int.left,
            RailDirection.Right => Vector2Int.right,
            _ => Vector2Int.zero
        };
    }

    RailDirection Opposite(RailDirection dir)
    {
        return dir switch
        {
            RailDirection.Up => RailDirection.Down,
            RailDirection.Down => RailDirection.Up,
            RailDirection.Left => RailDirection.Right,
            RailDirection.Right => RailDirection.Left,
            _ => dir
        };
    }
}
