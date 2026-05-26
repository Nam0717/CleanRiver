using UnityEngine;
using System.Collections.Generic;

public class PlantPlacement : MonoBehaviour
{
    [Header("Plant")]
    public List<GameObject> plantPrefabs;
    public GameObject previewPrefab;
    public LayerMask groundMask;

    [Header("Placement Settings")]
    public float placementDistance = 6f;
    public KeyCode toggleKey = KeyCode.F;

    [Header("Preview Color")]
    public Color canPlaceColor = new Color(0f, 1f, 0f, 0.5f);
    public Color cannotPlaceColor = new Color(1f, 0f, 0f, 0.5f);

    [Header("Distance Check")]
    public float minDistanceBetweenPlants = 2f;
    public LayerMask plantLayerMask;

    [Header("Preview Stability")]
    public float groundStickTime = 0.15f;

    private Camera cam;
    private bool isPlacing = false;
    private bool canPlace = false;

    private GameObject previewInstance;
    private Renderer[] previewRenderers;

    private bool hasLastValidGround;
    private Vector3 lastValidGroundPos;
    private Vector3 lastValidGroundNormal;

    private float groundStickCounter;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleInput();

        if (isPlacing)
        {
            UpdatePreviewPosition();
            UpdatePreviewColor();
        }
    }

    void HandleInput()
    {
        // 🔥 chỉ toggle bằng phím F
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePlacement();
        }

        if (!isPlacing) return;

        // ✅ Chuột trái để trồng
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            PlacePlant();
        }
    }

    void TogglePlacement()
    {
        if (isPlacing)
        {
            CancelPlacement();
            return;
        }

        isPlacing = true;
        previewInstance = Instantiate(previewPrefab);
        previewRenderers = previewInstance.GetComponentsInChildren<Renderer>();

        // reset trạng thái preview
        hasLastValidGround = false;
        groundStickCounter = 0f;
        canPlace = false;
    }

    void UpdatePreviewPosition()
    {
        if (previewInstance == null) return;
        if (InteractionRaycaster.Instance == null) return;

        canPlace = false;

        Ray ray = InteractionRaycaster.Instance.GetRay();
        bool hasHit = InteractionRaycaster.Instance.HasHit;

        // ===== RAY HIT GROUND =====
        if (hasHit)
        {
            RaycastHit hit = InteractionRaycaster.Instance.CurrentHit;

            bool isGround =
                (groundMask.value & (1 << hit.collider.gameObject.layer)) != 0;

            if (isGround)
            {
                groundStickCounter = groundStickTime;

                lastValidGroundPos = hit.point;
                lastValidGroundNormal = hit.normal;
                hasLastValidGround = true;
            }
        }

        // ===== GROUND MODE (CÒN BUFFER) =====
        if (hasLastValidGround && groundStickCounter > 0f)
        {
            groundStickCounter -= Time.deltaTime;

            previewInstance.transform.position = lastValidGroundPos;
            previewInstance.transform.rotation =
                Quaternion.FromToRotation(Vector3.up, lastValidGroundNormal);

            bool hasNearbyPlant = Physics.OverlapSphere(
                lastValidGroundPos,
                minDistanceBetweenPlants,
                plantLayerMask
            ).Length > 0;

            bool hasEnoughSeeds =
                GameManager.Instance != null &&
                GameManager.Instance.CanPlant();

            canPlace = !hasNearbyPlant && hasEnoughSeeds;
            return;
        }

        // ===== FREE MODE (KHÔNG XUYÊN GROUND) =====
        Vector3 freePos = ray.origin + ray.direction * placementDistance;

        // Ray phụ bắn xuống để clamp (chống xuyên ground)
        if (Physics.Raycast(
            freePos + Vector3.up * 2f,
            Vector3.down,
            out RaycastHit downHit,
            5f,
            groundMask))
        {
            freePos.y = Mathf.Max(freePos.y, downHit.point.y + 0.02f);
        }

        previewInstance.transform.position = freePos;
        previewInstance.transform.rotation =
            Quaternion.LookRotation(-ray.direction, Vector3.up);

        hasLastValidGround = false;
        canPlace = false;
    }

    void UpdatePreviewColor()
    {
        if (previewRenderers == null) return;

        Color targetColor = canPlace ? canPlaceColor : cannotPlaceColor;

        foreach (Renderer r in previewRenderers)
        {
            foreach (Material mat in r.materials)
            {
                mat.color = targetColor;
            }
        }
    }

    void PlacePlant()
    {
        if (plantPrefabs == null || plantPrefabs.Count == 0) return;

        if (GameManager.Instance == null || !GameManager.Instance.CanPlant())
        {
            Debug.Log("Not enough seeds!");
            return;
        }

        int randomIndex = Random.Range(0, plantPrefabs.Count);
        GameObject selectedPlant = plantPrefabs[randomIndex];

        GameObject plant = Instantiate(
            selectedPlant,
            previewInstance.transform.position,
            previewInstance.transform.rotation
        );

        // Gán layer cho cây
        int plantLayer = LayerMask.NameToLayer("Plant");
        if (plantLayer != -1)
        {
            plant.layer = plantLayer;
        }

        GameManager.Instance.ConsumeSeeds();
    }

    void CancelPlacement()
    {
        isPlacing = false;
        canPlace = false;

        hasLastValidGround = false;
        lastValidGroundPos = Vector3.zero;
        lastValidGroundNormal = Vector3.up;

        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }
    }
}
