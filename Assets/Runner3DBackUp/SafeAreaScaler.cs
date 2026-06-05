using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class SafeAreaScaler : MonoBehaviour
{
    [Header("Design reference (px)")]
    public Vector2 designResolution = new Vector2(390f, 844f);

    [Header("Optional limits")]
    public float minScale = 0.5f;
    public float maxScale = 2f;

    [Tooltip("If true, center the Scaler inside its parent safe-area rect")]
    public bool centerInParent = true;

    RectTransform rt;            // this Scaler
    RectTransform parentRt;      // SA (parent that SafeArea adjusted)
    Vector2 lastParentSize = Vector2.zero;
    Vector2 lastDesign = Vector2.zero;

    void OnEnable()
    {
        rt = GetComponent<RectTransform>();
        if (transform.parent != null)
            parentRt = transform.parent as RectTransform;

        ApplyScale();
    }

    void Update()
    {
        // Update when parent size changes or designResolution changed in editor/runtime
        if (parentRt == null && transform.parent != null)
            parentRt = transform.parent as RectTransform;

        Vector2 parentSize = (parentRt != null) ? parentRt.rect.size : new Vector2(Screen.width, Screen.height);
        if (parentSize != lastParentSize || designResolution != lastDesign)
            ApplyScale();
    }

    void ApplyScale()
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();

        Vector2 parentSize = (parentRt != null) ? parentRt.rect.size : new Vector2(Screen.width, Screen.height);

        if (designResolution.x <= 0 || designResolution.y <= 0)
        {
            Debug.LogWarning("Scaler: invalid designResolution.");
            return;
        }

        // compute scale factors (how much to scale the design to fit parent safe area)
        float scaleX = parentSize.x / designResolution.x;
        float scaleY = parentSize.y / designResolution.y;

        // choose the smaller to preserve aspect ratio and avoid cropping
        float chosen = Mathf.Min(scaleX, scaleY);

        // clamp
        chosen = Mathf.Clamp(chosen, minScale, maxScale);

        // apply
        rt.localScale = Vector3.one * chosen;

        // optional: center the scaled canvas in parent safe area
        if (centerInParent && parentRt != null)
        {
            // ensure pivot center for nice centering
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }

        lastParentSize = parentSize;
        lastDesign = designResolution;
    }
}
