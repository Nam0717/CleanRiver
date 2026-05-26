using UnityEngine;

public class MyCanvas : MonoBehaviour
{
    public CanvasGroup Tutal1;
    public CanvasGroup Tutal2;
    public void Start()
    {
        CVGRON(Tutal1, true);
        CVGROFF(Tutal2, false);
    }
    public void HideCanvas()
    {
        CVGROFF(Tutal1, false);
        CVGROFF(Tutal2, false);
    }
    public void CVGRON(CanvasGroup Gr, bool fl)
    {
        Gr.alpha = 1;
        Gr.interactable = true;
        Gr.blocksRaycasts = true;
    }
    public void CVGROFF ( CanvasGroup Gr,bool fl)
    {
        Gr.alpha = 0;
        Gr.interactable = false;
        Gr.blocksRaycasts = false;
    }
    public void OffTutal1()
    {
        CVGROFF(Tutal1, false);
        CVGRON(Tutal2, true);
    }
    public void OffAll()
    {
        HideCanvas();
    }
    public void ClickTutal1()
    {
        OffTutal1();
    }
    public void ClickTutal2()
    {
        OffAll();
    }
  
}
