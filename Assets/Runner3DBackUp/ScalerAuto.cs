using UnityEngine;

[ExecuteAlways]
public class ScalerAuto : MonoBehaviour
{
    [Header("Design Resolution (px)")]
    public Vector2 designResolution = new Vector2(390f, 844f);

    RectTransform rt;
    RectTransform parentRt;

    void OnEnable()
    {
        rt = GetComponent<RectTransform>();
        parentRt = transform.parent as RectTransform;
        ApplyScale();
    }

    void Update()
    {
        ApplyScale();
    }

    void ApplyScale()
    {
        if (rt == null || parentRt == null) return;

        Vector2 parentSize = parentRt.rect.size;   // size của SafeArea
        if (parentSize.x <= 0 || parentSize.y <= 0) return;

        float scaleX = parentSize.x / designResolution.x;
        float scaleY = parentSize.y / designResolution.y;

        float scale = Mathf.Min(scaleX, scaleY);

        // scale toàn bộ UI theo tỉ lệ nhỏ hơn để không bị méo
        rt.localScale = Vector3.one * scale;

        // giữ cho Scaler ở giữa safe-area
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        // kích thước gốc (khung thiết kế) tạo bằng code
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, designResolution.x);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, designResolution.y);
    }
}
