using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 2f;

    private GameObject heldTrash;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldTrash == null)
            {
                TryPickup();
            }
        }
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Trash"))
            {
                heldTrash = hit.gameObject;

                heldTrash.GetComponent<Rigidbody>().isKinematic = true;
                heldTrash.transform.position = holdPoint.position;
                heldTrash.transform.parent = holdPoint;

                break;
            }
        }
    }

    public void DropTrashOnTruck(Transform truckPoint)
    {
        if (heldTrash != null)
        {
            heldTrash.transform.parent = null;
            heldTrash.transform.position = truckPoint.position;

            heldTrash.GetComponent<Rigidbody>().isKinematic = false;

            heldTrash = null;

            TrashManager.instance.CollectTrash();
        }
    }
    public bool HasTrash()
    {
        return heldTrash != null;
    }

    public GameObject ReleaseTrash()
    {
        GameObject temp = heldTrash;
        heldTrash = null;
        temp.transform.parent = null;
        return temp;
    }
}