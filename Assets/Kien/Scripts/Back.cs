using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    public void onBack()
    {
        SceneManager.LoadScene("MainScene");
    }
}
