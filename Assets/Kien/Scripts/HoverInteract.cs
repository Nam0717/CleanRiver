using UnityEngine;
using UnityEngine.SceneManagement;

public class HoverInteract : MonoBehaviour
{
    public MeshRenderer[] renderers;
    public Material outlineMaterial;
    public string targetScene = "Kien";

    void OnMouseEnter()
    {
        foreach (var r in renderers)
        {
            var mats = r.materials;
            if (mats.Length == 1)
            {
                r.materials = new Material[] { mats[0], outlineMaterial };
            }
        }
    }

    void OnMouseExit()
    {
        foreach (var r in renderers)
        {
            var mats = r.materials;
            if (mats.Length > 1)
            {
                r.materials = new Material[] { mats[0] };
            }
        }
    }

    void OnMouseDown()
    {
        SceneManager.LoadScene(targetScene);
    }
}