using UnityEngine;

public class TrashTruck : MonoBehaviour
{
    public Transform dumpPoint;

    public int columns = 3;      // số rác mỗi hàng
    public float spacing = 0.6f; // khoảng cách giữa các rác
    public float rowHeight = 0.5f; // cao mỗi tầng

    private int trashCount = 0;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayerPickup player = other.GetComponent<PlayerPickup>();
                if (player != null)
                {
                    DropTrash(player);
                }
            }
        }
    }

    void DropTrash(PlayerPickup player)
    {
        if (player.HasTrash())
        {
            GameObject trash = player.ReleaseTrash();

            int row = trashCount / columns;
            int column = trashCount % columns;

            Vector3 offset = new Vector3(
                column * spacing,
                row * rowHeight,
                0
            );

            trash.transform.position = dumpPoint.position + offset;
            trash.transform.parent = dumpPoint;

            trash.GetComponent<Rigidbody>().isKinematic = true;

            trashCount++;

            TrashManager.instance.CollectTrash();
        }
    }
}