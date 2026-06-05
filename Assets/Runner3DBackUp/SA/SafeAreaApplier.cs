using UnityEngine;

/// <summary>
/// Attach to a UI RectTransform that you want to clip/move into the device safe area (notch, rounded corners, home bar).
/// It sets anchorMin/anchorMax according to Screen.safeArea so children stay inside the visible area.
/// Works on both iOS and Android. Reapplies on orientation/size change.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaApplier : MonoBehaviour
{
    RectTransform mRect;
    Rect mLastSafeArea = Rect.zero;
    ScreenOrientation mLastOrientation = ScreenOrientation.AutoRotation;

    void Awake()
    {
        mRect = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void OnEnable()
    {
        mLastSafeArea = Rect.zero;
        ApplySafeArea();
    }

    void Update()
    {
        // Re-apply if safeArea changed (orientation/resize)
        if (mLastSafeArea != Screen.safeArea || mLastOrientation != Screen.orientation)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        Rect safe = Screen.safeArea;

        // Convert safe area rectangle from pixels to normalized anchor coordinates (0..1)
        Vector2 anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
        Vector2 anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);

        // Apply to RectTransform anchors
        mRect.anchorMin = anchorMin;
        mRect.anchorMax = anchorMax;

        // Optionally zero out offsets so children align to anchors exactly
        mRect.offsetMin = Vector2.zero;
        mRect.offsetMax = Vector2.zero;

        mLastSafeArea = safe;
        mLastOrientation = Screen.orientation;
    }
}
