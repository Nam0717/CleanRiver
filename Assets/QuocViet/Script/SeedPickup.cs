using UnityEngine;

public class SeedPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public int seedAmount = 1;

    [Header("Auto Destroy")]
    public float lifeTime = 5f; // 👈 sau 5 giây tự biến mất

    [Header("UI")]
    public GameObject pickupUI; // Image bạn thiết kế (con của seed)

    private bool isLookingAt = false;
    private float timer;

    void Start()
    {
        if (pickupUI != null)
            pickupUI.SetActive(false);

        timer = lifeTime;
    }

    void Update()
    {
        HandleLifetime();
        CheckRaycast();

        if (isLookingAt && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    void HandleLifetime()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void CheckRaycast()
    {
        isLookingAt = false;

        if (InteractionRaycaster.Instance == null) return;
        if (!InteractionRaycaster.Instance.HasHit) return;

        RaycastHit hit = InteractionRaycaster.Instance.CurrentHit;

        if (hit.collider.gameObject == gameObject)
        {
            isLookingAt = true;
        }

        if (pickupUI != null)
            pickupUI.SetActive(isLookingAt);
    }

    void Pickup()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddSeeds(seedAmount);
        }

        Destroy(gameObject);
    }
}
