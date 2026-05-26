using UnityEngine;

public class WaterTrashObject : MonoBehaviour
{
    public enum TrashCategory { Plastic, Metal, Organic }
    
    [Header("Cài đặt loại rác")]
    public TrashCategory category; // Đã là public để script WaterBin truy cập
    public int requiredToolID; 

    [Header("Cài đặt trôi nổi")]
    public float moveSpeed = 2f;
    public Vector3 flowDirection = Vector3.right;
    public float bobSpeed = 2f;
    public float bobHeight = 0.15f;

    [HideInInspector] public bool isOnWater = true; // Chuyển sang public để script Player kiểm tra
    private bool isHeld = false;
    private Rigidbody rb;
    private float startY;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        // Khóa Y để rác nổi trên mặt nước Trigger
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        startY = transform.position.y;
    }

    void Update() {
        if (isOnWater && !isHeld) {
            // Trôi ngang ổn định theo dòng chảy
            transform.Translate(flowDirection * moveSpeed * Time.deltaTime, Space.World);
            
            // Hiệu ứng nhấp nhô giả lập sóng nước
            float newY = startY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    // Hàm được gọi khi dùng phím 4 (Nhặt tay)
    public void PickUpToHand(Transform holdPoint) {
        isHeld = true;
        rb.isKinematic = true; // Tắt vật lý khi đang trên tay
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Hàm được gọi khi Click chuột trái để ném
    public void Throw(Vector3 direction, float force) {
        isHeld = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None; // Mở khóa để rơi tự do khi ném
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    // Hàm được gọi khi dùng công cụ 1,2,3 để vớt về bờ
    public void BeCollected(Vector3 shorePos) {
        isOnWater = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None; // Mở khóa Y để nằm trên mặt đất
        transform.position = shorePos + Vector3.up * 1f;
    }

    public bool IsHeld() => isHeld;
}