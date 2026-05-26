using UnityEngine;

public class InteractionRaycaster : MonoBehaviour
{
    public static InteractionRaycaster Instance;

    [Header("Ray Settings")]
    public Camera cam;
    public float rayDistance = 6f;
    public LayerMask interactMask;

    public RaycastHit CurrentHit { get; private set; }
    public bool HasHit { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        CastRay();
    }

    void CastRay()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, interactMask))
        {
            HasHit = true;
            CurrentHit = hit;
        }
        else
        {
            HasHit = false;
        }
    }
    public Ray GetRay()
    {
        return cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

}
