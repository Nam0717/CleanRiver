using UnityEngine;
using UnityEngine.UI;

public class GuideUI : MonoBehaviour
{
    public Image guideImage;          // Image để hiển thị hình
    public Sprite[] guideSprites;     // danh sách hình hướng dẫn

    private int index = 0;

    void Start()
    {
        ShowImage(0);
    }

    void ShowImage(int i)
    {
        index = Mathf.Clamp(i, 0, guideSprites.Length - 1);

        if (guideImage != null && guideSprites.Length > 0)
        {
            guideImage.sprite = guideSprites[index];
        }
    }

    public void Next()
    {
        if (guideSprites.Length == 0) return;

        index++;
        if (index >= guideSprites.Length) index = 0; // quay vòng
        ShowImage(index);
    }

    public void Prev()
    {
        if (guideSprites.Length == 0) return;

        index--;
        if (index < 0) index = guideSprites.Length - 1; // quay vòng
        ShowImage(index);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
