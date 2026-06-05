using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadSceneManager : MonoBehaviour
{
  public void NextLevel2()
    {
        FindAnyObjectByType<FadeManager>().FadeAndLoadScene("2");
    }
    public void NextLevel3()
    {
        FindAnyObjectByType<FadeManager>().FadeAndLoadScene("2");
    }
    public void BackMenu()
    {
        FindAnyObjectByType<FadeManager>().FadeAndLoadScene("Menu");
        Time.timeScale = 1f;
    }
}
