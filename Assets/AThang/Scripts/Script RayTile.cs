using System.Collections.Generic;
using UnityEngine;

public enum RailDirection
{
    Up, Down, Left, Right
}

public class RailTile : MonoBehaviour
{
    public Vector2Int gridPos;

    public RailDirection entry;
    public RailDirection exit;

    [Tooltip("Local path points")]
    public List<Vector3> localPath;

    public List<Vector3> GetWorldPath(bool reverse)
    {
        List<Vector3> world = new List<Vector3>();

        if (!reverse)
        {
            foreach (var p in localPath)
                world.Add(transform.TransformPoint(p));
        }
        else
        {
            for (int i = localPath.Count - 1; i >= 0; i--)
                world.Add(transform.TransformPoint(localPath[i]));
        }

        return world;
    }

    void OnDrawGizmos()
    {
        if (localPath == null || localPath.Count < 2) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < localPath.Count - 1; i++)
        {
            Vector3 a = transform.TransformPoint(localPath[i]);
            Vector3 b = transform.TransformPoint(localPath[i + 1]);

            Gizmos.DrawLine(a, b);
            Gizmos.DrawSphere(a, 0.05f);
        }

        // vẽ điểm cuối
        Gizmos.DrawSphere(
            transform.TransformPoint(localPath[^1]),
            0.05f
        );
    }
    public bool HasDirection(RailDirection dir)
    {
        return entry == dir || exit == dir;
    }

    public RailDirection GetOther(RailDirection dir)
    {
        if (entry == dir) return exit;
        if (exit == dir) return entry;
        return dir; // fallback (không nên xảy ra)
    }


}
