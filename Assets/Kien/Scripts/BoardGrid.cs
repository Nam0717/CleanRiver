using UnityEngine;

public class BoardGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public float cellSize = 1f;

    public GameObject cellPrefab;

    public GridCell[,] grid;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new GridCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);

                GameObject cellObj = Instantiate(cellPrefab, position, Quaternion.identity, transform);

                GridCell cell = cellObj.GetComponent<GridCell>();
                cell.x = x;
                cell.z = z;

                grid[x, z] = cell;
            }
        }
    }

    public Vector3 GetSnapPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int z = Mathf.RoundToInt(worldPos.z / cellSize);

        x = Mathf.Clamp(x, 0, width - 1);
        z = Mathf.Clamp(z, 0, height - 1);

        return new Vector3(x * cellSize, 0.5f, z * cellSize);
    }
}