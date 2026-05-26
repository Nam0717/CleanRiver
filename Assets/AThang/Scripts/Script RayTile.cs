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

    public List<Vector3> GetWorldPath()
    {
        List<Vector3> world = new List<Vector3>();
        foreach (var p in localPath)
            world.Add(transform.TransformPoint(p));
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

}
