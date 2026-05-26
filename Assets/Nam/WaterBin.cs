using UnityEngine;

public class WaterBin : MonoBehaviour
{
    public WaterTrashObject.TrashCategory binType;

    public bool DepositTrash(WaterTrashObject trash) {
        if (trash == null) return false;

        if (trash.category == binType) {
            Debug.Log("<color=green>CHÍNH XÁC!</color>");
            // Manager sẽ xử lý việc cộng điểm 15/15
            return true;
        } else {
            Debug.Log("<color=red>SAI THÙNG!</color>");
            return false;
        }
    }
}