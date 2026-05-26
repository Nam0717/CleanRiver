using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TrashManager : MonoBehaviour
{
    public static TrashManager instance;

    public int trashCount = 0;
    public int totalTrash = 15;
    public string nextSceneName;

    public TMP_Text trashText; // kéo text vào đây

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void CollectTrash()
    {
        trashCount++;
        UpdateUI();

        if (trashCount >= totalTrash)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void UpdateUI()
    {
        if (trashText != null)
        {
            trashText.text = "Trash: " + trashCount + "/" + totalTrash;
        }
    }
}
