using UnityEngine;

public class CutsceneEventReceiver : MonoBehaviour
{
    [Header("Objects To Toggle")]
    public Behaviour[] componentsToDisable;  // script, collider, controller...
    public GameObject[] objectsToDisable;    // object nguyên khối

    // Animation Event gọi
    public void DisableTargets()
    {
        // Tắt component
        if (componentsToDisable != null)
        {
            foreach (var c in componentsToDisable)
            {
                if (c != null) c.enabled = false;
            }
        }

        // Tắt object
        if (objectsToDisable != null)
        {
            foreach (var obj in objectsToDisable)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
    }

    // Animation Event gọi
    public void EnableTargets()
    {
        // Bật component
        if (componentsToDisable != null)
        {
            foreach (var c in componentsToDisable)
            {
                if (c != null) c.enabled = true;
            }
        }

        // Bật object
        if (objectsToDisable != null)
        {
            foreach (var obj in objectsToDisable)
            {
                if (obj != null) obj.SetActive(true);
            }
        }
    }
}
