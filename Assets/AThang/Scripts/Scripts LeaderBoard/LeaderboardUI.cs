using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardUI : MonoBehaviour
{
    public Transform content;
    public GameObject rowPrefab;

    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public int move;
    }

    [Header("Fake Players")]
    public List<PlayerData> fakePlayers = new List<PlayerData>();

    public void ShowLeaderboard(int playerMove)
    {
        ClearBoard();

        List<PlayerData> list = new List<PlayerData>(fakePlayers);

        list.Add(new PlayerData()
        {
            name = SaveData.GetName(),
            move = playerMove
        });

        list = list.OrderBy(x => x.move).ToList();

        for (int i = 0; i < list.Count; i++)
        {
            CreateRow(i + 1, list[i].name, list[i].move);
        }
    }

    void CreateRow(int rank, string name, int move)
    {
        GameObject obj = Instantiate(rowPrefab, content);
        obj.GetComponent<LeaderboardRow>().Setup(rank, name, move);
    }

    void ClearBoard()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}