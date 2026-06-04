using UnityEngine;

public class GridRailManager : MonoBehaviour
{
    public int width = 4;
    public int height = 4;

    public Vector3 origin;

    public RailTile[,] grid;

    void Awake()
    {
        grid = new RailTile[width, height];
    }

    public void Register(RailTile tile)
    {
        grid[tile.gridPos.x, tile.gridPos.y] = tile;
    }

    public RailTile GetTile(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
            return null;

        return grid[pos.x, pos.y];
    }
}