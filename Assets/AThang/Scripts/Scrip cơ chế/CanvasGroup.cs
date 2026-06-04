using TMPro;
using UnityEngine;

public class OnOffUI : MonoBehaviour
{
    public UIPanelFade Option;
    public GameObject Run;
    public GameObject Timer;
    public GameObject ButtonSpeed;
    public GameObject MoveBlock;

    int speedLevel = 0;
    public TMP_Text speedText;
    private void Start()
    {

        speedText.text = "X1";
        Time.timeScale = 1f;
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
        speedLevel = (speedLevel + 1) % 3;

        switch (speedLevel)
        {
            case 0:
                Time.timeScale = 1f;
                speedText.text = "X1";
                break;

            case 1:
                Time.timeScale = 2f;
                speedText.text = "X2";
                break;

            case 2:
                Time.timeScale = 3f;
                speedText.text = "X3";
                break;
        }
    }

}
