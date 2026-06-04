using UnityEngine;

public class OnOffUI : MonoBehaviour
{
    public UIPanelFade Option;
    public GameObject Run;
    public GameObject Timer;
    public GameObject ButtonSpeed;
    public GameObject MoveBlock;

    int speedLevel = 0;
    public GameObject speed1;
    public GameObject speed2;
    public GameObject speed3;
    private void Start()
    {
        speed1.SetActive(true);
        speed2.SetActive(false);
        speed3.SetActive(false);
    }
    public void OnOption()
    {
        Option.Show();
        Run.SetActive(false);
        Timer.SetActive(false);
        ButtonSpeed.SetActive(false);
        MoveBlock.SetActive(false);
    }
    public void CloseOption()
    {
        Option.Hide();
        Run.SetActive(true);
        Timer.SetActive(true);
        ButtonSpeed.SetActive(true);
        MoveBlock.SetActive(true);
    }
    public void SpeedTime()
    {
        speedLevel++;

        if (speedLevel > 2)
        {
            speedLevel = 0;
        }

        switch (speedLevel)
        {
            case 0:
                Time.timeScale = 1f;
                speed2.SetActive(false);
                speed3.SetActive(false);
                break;

            case 1:
                Time.timeScale = 2f;
                speed2.SetActive(true);
     
                break;

            case 2:
                Time.timeScale = 3f;
                speed2.SetActive(true);
                speed3.SetActive(true);
                break;
        }
    }

}
