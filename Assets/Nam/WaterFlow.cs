using UnityEngine;

public class WaterFlow : MonoBehaviour
{
    public float flowSpeed = 0.5f;
    private MeshRenderer meshRenderer;

    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update() {
        // Di chuyển Texture của nước theo thời gian để tạo cảm giác chảy
        float offset = Time.time * flowSpeed;
        meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
    }
}