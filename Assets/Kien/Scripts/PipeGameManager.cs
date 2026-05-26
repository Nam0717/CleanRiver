using System.Collections.Generic;
using UnityEngine;

public class PipeGameManager : MonoBehaviour
{
    public Connector startConnector;
    public GameObject endPipe; // đổi sang endPipe

    public void CheckWin()
    {
        Debug.Log("===== ALL CONNECTIONS =====");

        foreach (var c in Connector.All)
        {
            if (c.IsConnected)
                Debug.Log(c.name + "  <-->  " + c.connectedTo.name);
            else
                Debug.Log(c.name + "  (NOT CONNECTED)");
        }

        Debug.Log("===========================");

        HashSet<Connector> visited = new HashSet<Connector>();
        Queue<Connector> queue = new Queue<Connector>();

        queue.Enqueue(startConnector);
        visited.Add(startConnector);

        while (queue.Count > 0)
    {
        Connector current = queue.Dequeue();
        Debug.Log("Visiting: " + current.name);

        if (current.GetComponentInParent<EndPipe>() != null)
        {
            Debug.Log("WIN 🎉");
            CutsceneController.Instance.PlayWinCutscene();
            return;
        }

        // 1️⃣ Đi sang connector đang nối
        if (current.connectedTo != null && !visited.Contains(current.connectedTo))
        {
            visited.Add(current.connectedTo);
            queue.Enqueue(current.connectedTo);
        }

        // 2️⃣ Đi sang connector khác cùng pipe
        Connector[] siblings = current.transform.root.GetComponentsInChildren<Connector>();

        foreach (var sibling in siblings)
        {
            if (sibling == current) continue;

            if (!visited.Contains(sibling))
            {
                visited.Add(sibling);
                queue.Enqueue(sibling);
            }
        }
    }

        Debug.Log("NOT CONNECTED ❌");
    }
}