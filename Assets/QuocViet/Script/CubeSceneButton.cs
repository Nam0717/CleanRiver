using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeSceneButton : MonoBehaviour
{
    public CubeFaceManager faceManager;

    public void LoadSceneFromTopFace()
    {
        int sceneIndex = faceManager.GetTopFaceSceneIndex();
        SceneManager.LoadScene(sceneIndex);
    }
}
