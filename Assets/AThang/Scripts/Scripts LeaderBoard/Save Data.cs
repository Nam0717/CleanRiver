using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static void SavePlayer(string name, int move)
    {
        PlayerPrefs.SetString("PlayerName", name);

        int bestMove = PlayerPrefs.GetInt("BestMove", 9999);

        if (move < bestMove)
            PlayerPrefs.SetInt("BestMove", move);

        PlayerPrefs.Save();
    }

    public static string GetName()
    {
        return PlayerPrefs.GetString("PlayerName", "YOU");
    }

    public static int GetBestMove()
    {
        return PlayerPrefs.GetInt("BestMove", 9999);
    }
}