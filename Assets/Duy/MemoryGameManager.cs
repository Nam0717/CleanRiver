using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager Instance;

    [Header("Score UI")]
public TextMeshProUGUI scoreText;
public TextMeshProUGUI scoreText2;
public TextMeshProUGUI resultText;

public int targetScore = 10;

private int score = 0;


    [Header("Tiles")]
    public List<MemoryTile> tiles;   // 5 ô

    [Header("Settings")]
    public float delayBetweenTiles = 0.8f;

    [Header("UI")]
    public GameObject playButton;
    public GameObject pannel;
    public GameObject pannel2;
    public GameObject homebutton;
    public GameObject Map;
    public GameObject text;
    public GameObject text2;

    [HideInInspector]
    public bool playerTurn = false;

    private List<int> sequence = new List<int>();
    private int playerIndex = 0;
    private bool gameStarted = false;

    // 🔥 SHUFFLE BAG
    private List<int> randomBag = new List<int>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerTurn = false;
    }

    // =========================
    // PLAY BUTTON
    // =========================
    public void OnPlayButtonClicked()
    {
        playButton.SetActive(false);
        pannel.SetActive(false);
        pannel2.SetActive(false);
        homebutton.SetActive(false);
        Map.SetActive(true);
        text.SetActive(false);
        text2.SetActive(true);
        StartGame();
    }

    void StartGame()
    {
        gameStarted = true;
        playerTurn = false;

        sequence.Clear();
        randomBag.Clear();

        AddRandomTile();
        StartCoroutine(PlaySequence());

        score = 0;
UpdateScoreUI();
ClearResultUI();


    }

    // =========================
    // SHUFFLE BAG RANDOM
    // =========================
    void AddRandomTile()
    {
        // Nếu bag rỗng → đổ lại 0..4 và trộn
        if (randomBag.Count == 0)
        {
            for (int i = 0; i < tiles.Count; i++)
                randomBag.Add(i);

            ShuffleBag();
        }

        int nextTile = randomBag[0];
        randomBag.RemoveAt(0);

        sequence.Add(nextTile);

        Debug.Log("Added tile: " + nextTile);
    }

    void ShuffleBag()
    {
        for (int i = 0; i < randomBag.Count; i++)
        {
            int rnd = Random.Range(i, randomBag.Count);
            int temp = randomBag[i];
            randomBag[i] = randomBag[rnd];
            randomBag[rnd] = temp;
        }
    }

    // =========================
    // PLAY SEQUENCE
    // =========================
    IEnumerator PlaySequence()
    {
        playerTurn = false;
        playerIndex = 0;

        yield return new WaitForSeconds(0.8f);

        foreach (int index in sequence)
        {
            tiles[index].PlayEffect();
            yield return new WaitForSeconds(delayBetweenTiles);
        }

        playerTurn = true;
    }

    // =========================
    // PLAYER INPUT
    // =========================
    public void PlayerChoose(int tileIndex)
    {
        if (!gameStarted || !playerTurn)
            return;

        if (tileIndex == sequence[playerIndex])
        {
            playerIndex++;

            // Hoàn thành chuỗi
            if (playerIndex >= sequence.Count)
{
    score++;
    UpdateScoreUI();

    playerTurn = false;
    AddRandomTile();
    StartCoroutine(PlaySequence());
}


        }
        else
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameStarted = false;
        playerTurn = false;

        playButton.SetActive(true);
        //pannel.SetActive(true);
        pannel2.SetActive(true);
        homebutton.SetActive(true);
        Map.SetActive(false);
        text.SetActive(true);
        text2.SetActive(false);
        ShowResultUI();

        Debug.Log("❌ GAME OVER | Score: " + (sequence.Count - 1));
        

    }

    void UpdateScoreUI()
{
    if (scoreText != null)
        scoreText.text = "Điểm: " + score + "/" + targetScore;
        scoreText2.text = "Điểm: " + score + "/" + targetScore;
}

void ClearResultUI()
{
    if (resultText != null)
        resultText.text = "";
}

void ShowResultUI()
{
    if (resultText == null) return;

    if (score >= targetScore)
        resultText.text = "THÀNH CÔNG";
    else
        resultText.text = "THẤT BẠI";
}


}
