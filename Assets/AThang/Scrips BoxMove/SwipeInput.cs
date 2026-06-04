using UnityEngine;

public class SwipeInput : MonoBehaviour
{
    Vector2 startPos;
    CubeController selectedCube;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(startPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                selectedCube = hit.transform.GetComponent<CubeController>();
            }
        }

        if (Input.GetMouseButtonUp(0) && selectedCube != null)
        {
            Vector2 endPos = Input.mousePosition;
            Vector2 delta = endPos - startPos;

            if (delta.magnitude < 50) return;

            Vector2Int dir;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                dir = delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            else
                dir = delta.y > 0 ? Vector2Int.up : Vector2Int.down;

            selectedCube.TryMove(dir);
            selectedCube = null;
        }
    }
}
