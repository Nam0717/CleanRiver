using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeFaceSceneLoader : MonoBehaviour
{
    public Transform cube; // kéo Cube vào đây

    public void LoadSceneByTopFace()
    {
        Vector3 up = cube.up;

        int sceneIndex = GetSceneIndexFromUpVector(up);

        Debug.Log("Top face scene index: " + sceneIndex);

        SceneManager.LoadScene(sceneIndex);
    }

    int GetSceneIndexFromUpVector(Vector3 up)
    {
        up = up.normalized;

        if (Vector3.Dot(up, Vector3.up) > 0.9f) return 0;   // Top
        if (Vector3.Dot(up, Vector3.down) > 0.9f) return 1; // Bottom
        if (Vector3.Dot(up, Vector3.right) > 0.9f) return 2;// Right
        if (Vector3.Dot(up, Vector3.left) > 0.9f) return 3; // Left
        if (Vector3.Dot(up, Vector3.forward) > 0.9f) return 4;// Front
        if (Vector3.Dot(up, Vector3.back) > 0.9f) return 5; // Back

        return 0;
    }
}
