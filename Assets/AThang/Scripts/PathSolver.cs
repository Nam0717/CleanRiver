using System.Collections.Generic;
using UnityEngine;

public class PathSolver : MonoBehaviour
{
    public RailTile startTile;
    public RailTile finishTile;
    public GridRailManager grid;
    public List<Vector3> finalPath = new List<Vector3>();
    public bool isCompletePath { get; private set; }


    public bool Solve()
    {

        finalPath.Clear();
        isCompletePath = false;

        RailTile current = startTile;
        RailDirection dir = startTile.exit;

        finalPath.AddRange(startTile.GetWorldPath(false));


        int safety = 0;

        while (true)
        {
            safety++;
            if (safety > 50) break;

            if (current == finishTile)
            {
                isCompletePath = true;
                break;
            }

            Vector2Int nextPos = current.gridPos + DirToVector(dir);
            RailTile next = grid.GetTile(nextPos);

            if (next == null)
            {
                Debug.Log("⛔ Đứt rail tại " + nextPos);
                break;
            }

            RailDirection incoming = Opposite(dir);

            if (!next.HasDirection(incoming))
            {
                Debug.Log("⛔ Sai hướng tại " + next.gridPos);
                break;
            }

            bool reversePath = (incoming == next.exit);
            finalPath.AddRange(next.GetWorldPath(reversePath));

            dir = next.GetOther(incoming);
            current = next;
        }

        // 👉 chỉ cần có path là cho chạy
        return finalPath.Count > 0;

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
