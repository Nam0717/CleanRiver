using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text moveText;
   
    [Header("Highlight YOU")]
    public Color textHighlightColor = Color.yellow;
    public Color bgHighlightColor = new Color(1f, 0.9f, 0.2f, 0.35f);
    public float highlightScale = 1.08f;

    public void Setup(int rank, string playerName, int move)
    {
        rankText.text = rank.ToString();
        nameText.text = playerName;
        moveText.text = move.ToString();

        if (playerName == "YOU")
        {
            rankText.color = textHighlightColor;
            nameText.color = textHighlightColor;
            moveText.color = textHighlightColor;

           

           
        }
    }
}