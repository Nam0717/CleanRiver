using System.Collections;
using UnityEngine;

public class CubeRoll : MonoBehaviour
{
    public CubeFaceManager faceManager;

    public float rollSpeed = 300f;

    private bool isRolling = false;
    private Vector2 swipeStart;

    void Update()
    {
        if (isRolling) return;

        // ===== KEYBOARD =====
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            TryRoll(Vector3.back);

        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            TryRoll(Vector3.forward);

        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            TryRoll(Vector3.right);

        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            TryRoll(Vector3.left);

        // ===== MOUSE / SWIPE =====
        if (Input.GetMouseButtonDown(0))
            swipeStart = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 swipeEnd = Input.mousePosition;
            Vector2 delta = swipeEnd - swipeStart;

            if (delta.magnitude < 50f) return; // ❗ chống click nhẹ

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                TryRoll(delta.x > 0 ? Vector3.left : Vector3.right);
            else
                TryRoll(delta.y > 0 ? Vector3.back : Vector3.forward);
        }
    }

    // ===============================
    // CHỈ 1 CỬA DUY NHẤT ĐỂ LĂN
    // ===============================
    void TryRoll(Vector3 direction)
    {
        if (isRolling) return;

        if (direction == Vector3.forward)
            faceManager.RollForward();
        else if (direction == Vector3.back)
            faceManager.RollBack();
        else if (direction == Vector3.right)
            faceManager.RollRight();
        else if (direction == Vector3.left)
            faceManager.RollLeft();

        StartCoroutine(Roll(direction));
    }

    IEnumerator Roll(Vector3 direction)
    {
        isRolling = true;

        float angle = 0f;

        Vector3 pivot =
            transform.position +
            (Vector3.down + direction) * 0.5f;

        Vector3 axis = Vector3.Cross(Vector3.up, direction);

        while (angle < 90f)
        {
            float step = rollSpeed * Time.deltaTime;
            transform.RotateAround(pivot, axis, step);
            angle += step;
            yield return null;
        }

        // Fix sai số vị trí
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z)
        );

        // Fix sai số rotation
        transform.rotation = Quaternion.Euler(
            Mathf.Round(transform.eulerAngles.x / 90) * 90,
            Mathf.Round(transform.eulerAngles.y / 90) * 90,
            Mathf.Round(transform.eulerAngles.z / 90) * 90
        );

        isRolling = false;
    }
}
