using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Sun Settings")]
    public Light sun;
    public float dayDuration = 120f; // 1 ngày = 120 giây

    [Header("Light Settings")]
    public float maxIntensity = 1f;
    public float minIntensity = 0.1f;

    public Gradient lightColor; // màu ánh sáng theo thời gian

    private float time;

    void Update()
    {
        time += Time.deltaTime / dayDuration;
        if (time > 1f) time = 0f;

        // Xoay mặt trời (ngày -> đêm)
        float sunRotation = Mathf.Lerp(-90f, 270f, time);
        sun.transform.rotation = Quaternion.Euler(sunRotation, -90f, 0f);

        // Cường độ ánh sáng
        float intensity = Mathf.Clamp01(Mathf.Cos(time * Mathf.PI * 2f) * 0.5f + 0.5f);
        sun.intensity = Mathf.Lerp(minIntensity, maxIntensity, intensity);

        // Màu ánh sáng
        sun.color = lightColor.Evaluate(time);
    }
}
