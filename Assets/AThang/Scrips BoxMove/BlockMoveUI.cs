using TMPro;
using UnityEngine;

public class MoveUI : MonoBehaviour
{
    public static MoveUI Instance;

    public TMP_Text moveText;

    int moveCount = 0;

    void Awake()
    {
        Instance = this;
        UpdateText();
    }

    public void AddMove()
    {
        moveCount++;
        UpdateText();
    }

    void UpdateText()
    {
        moveText.text = "Move : " + moveCount;
    }
    public int GetMoveCount()
    {
        return moveCount;
    }

}