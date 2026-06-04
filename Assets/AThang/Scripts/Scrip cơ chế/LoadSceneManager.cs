using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadSceneManager : MonoBehaviour
{
  public void NextLevel2()
    {
        SceneManager.LoadScene("Level 2");
    }
    public void NextLevel3()
    {
        SceneManager.LoadScene("Level 3");
    }

}
