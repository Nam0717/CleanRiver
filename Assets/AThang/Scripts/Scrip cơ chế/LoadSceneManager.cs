using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadSceneManager : MonoBehaviour
{
    public GameObject Oiton;
    private void Start()
    {
        Oiton.SetActive(false);
    }
    public void NextLevel2()
    {
        FindAnyObjectByType<FadeManager>().FadeAndLoadScene("2");
    }
    public void NextLevel3()
    {
        FindAnyObjectByType<FadeManager>().FadeAndLoadScene("MinigameAnKeo");
    }
    public void BackMenu()
    {
        FindAnyObjectByType<FadeManager>().FadeAndLoadScene("Menu");
        Time.timeScale = 1f;
    }
    public void AMinigameAnKeo()
    {
        FindAnyObjectByType<FadeManager>().FadeAndLoadScene("MinigameAnKeo");
        Time.timeScale = 1f;
    }

    public void PaneOpttion()
    {
        Oiton.SetActive(true);
    }
    public void CLoseOption()
    {
        Oiton.SetActive(false);
    }
}
