using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterPlayerAction : MonoBehaviour
{
    [Header("Cài đặt")]
    public int currentToolID = 0; 
    public float reachDistance = 15f; 
    public float throwForce = 10f;
    public Transform shorePoint;
    public Transform holdPoint;
    public Camera playerCam;

    [Header("Cài đặt Nhặt tay (ID 4)")]
    public float pickupRadius = 5f; // Vùng kích cỡ cố định để nhặt đồ (Tính từ shorePoint)

    [Header("Cài đặt Kích cỡ khi cầm")]
    public Vector3 heldItemScale = new Vector3(0.3f, 0.3f, 0.3f); 
    private Vector3 originalScale; 

    [Header("Lớp va chạm")]
    public LayerMask layerToHit; 

    [Header("Hiển thị & UI")]
    public GameObject[] toolVisuals; 
    public GameObject interactPrompt; 
    public TextMeshProUGUI binNameText; 
    
    [Header("Hotbar UI")]
    public Image[] toolSlots; 
    public Color activeColor = Color.red;
    public Color normalColor = Color.white;

    private WaterTrashObject heldTrash;
    private WaterManager waterMan; 

    void Start() {
        waterMan = FindObjectOfType<WaterManager>();
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (binNameText != null) binNameText.text = ""; 
        UpdateToolUI(); 
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { DropHeldTrash(); SelectTool(0, "VỢT"); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { DropHeldTrash(); SelectTool(1, "XÔ"); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { DropHeldTrash(); SelectTool(2, "CẦN CÂU"); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { SelectTool(4, "NHẶT TAY"); }

        CheckForBinFocus();

        if (Input.GetMouseButtonDown(0)) {
            // ID 4 là bàn tay
            if (currentToolID == 4) HandleManualAction();
            else PerformCatch();
        }

        if (Input.GetKeyDown(KeyCode.F) && heldTrash != null) {
            HandleDeposit();
        }
    }

    // --- SỬA HÀM NHẶT TAY: Không cho ném và thêm giới hạn vùng ---
    void HandleManualAction() {
        // Chỉ cho phép nhặt nếu chưa cầm đồ
        if (heldTrash == null) {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, reachDistance, layerToHit)) {
                WaterTrashObject trash = hit.collider.GetComponent<WaterTrashObject>();
                
                if (trash != null) {
                    // Kiểm tra khoảng cách so với shorePoint
                    float distanceToShore = Vector3.Distance(trash.transform.position, shorePoint.position);
                    
                    if (distanceToShore <= pickupRadius) {
                        heldTrash = trash;
                        originalScale = heldTrash.transform.localScale;
                        heldTrash.PickUpToHand(holdPoint);
                        heldTrash.transform.localScale = heldItemScale;
                        Debug.Log("Đã nhặt rác!");
                    } else {
                        Debug.Log("Rác quá xa bờ, không thể nhặt tay!");
                    }
                }
            }
        } 
        // ĐÃ XÓA logic ném đồ (Else) để không thể ném khi dùng tay
        else {
            Debug.Log("Không thể ném bằng tay!");
        }
    }

    // ... (Các hàm còn lại giữ nguyên không đổi) ...
    void CheckForBinFocus() {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, reachDistance, layerToHit)) {
            WaterBin bin = hit.collider.GetComponent<WaterBin>();
            if (bin != null) {
                if (binNameText != null) binNameText.text = "THÙNG RÁC: " + bin.binType.ToString().ToUpper();
                if (interactPrompt != null && heldTrash != null) interactPrompt.SetActive(true);
            } else ClearBinUI();
        } else ClearBinUI();
    }

    void ClearBinUI() {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (binNameText != null) binNameText.text = "";
    }

    void UpdateToolUI() {
        if (toolSlots == null || toolSlots.Length < 4) return;
        toolSlots[0].color = (currentToolID == 0) ? activeColor : normalColor;
        toolSlots[1].color = (currentToolID == 1) ? activeColor : normalColor;
        toolSlots[2].color = (currentToolID == 2) ? activeColor : normalColor;
        toolSlots[3].color = (currentToolID == 4) ? activeColor : normalColor;
    }

    void SelectTool(int id, string toolName) {
        currentToolID = id;
        for (int i = 0; i < toolVisuals.Length; i++) {
            if(toolVisuals[i] != null) toolVisuals[i].SetActive(i == id);
        }
        UpdateToolUI();
    }

    void HandleDeposit() {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, reachDistance, layerToHit)) {
            WaterBin bin = hit.collider.GetComponent<WaterBin>();
            if (bin != null) {
                if (bin.DepositTrash(heldTrash)) {
                    if (waterMan != null) waterMan.AddDepositProgress(); 
                    Destroy(heldTrash.gameObject);
                    heldTrash = null;
                } else if (waterMan != null) waterMan.ShowFeedback(false);
            }
        }
    }

    void DropHeldTrash() {
        if (heldTrash != null) {
            heldTrash.transform.localScale = originalScale;
            heldTrash.Throw(Vector3.down, 0f); 
            heldTrash = null;
        }
    }

    void PerformCatch() {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, reachDistance, layerToHit)) {
            WaterTrashObject trash = hit.collider.GetComponent<WaterTrashObject>();
            if (trash != null && trash.requiredToolID == currentToolID) {
                trash.BeCollected(shorePoint.position);
                if (waterMan != null) waterMan.AddCleanProgress(); 
            }
        }
    }
}