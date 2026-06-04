using System.Collections.Generic;
using UnityEngine;

public class TrainMover : MonoBehaviour
{
    public GameObject Timer;
    public GameObject Run;
    public GameObject TimerSpeed;
    public GameObject Move;
    public GameObject Option;

    public float speed = 2f;
    public FadePanel PanelWin;
    private List<Vector3> path;
    private int index = 0;
    private bool moving = false;

    public LeaderboardUI leaderboard;

    public void StartMoving(List<Vector3> newPath)
    {
        path = newPath;
        index = 0;
        moving = true;
        transform.position = path[0];
    }

    void Update()
    {
        {
            if (!moving || path == null || index >= path.Count - 1) return;

            Vector3 current = transform.position;
            Vector3 target = path[index];

            Vector3 moveDir = (target - current).normalized;
            transform.position += moveDir * speed * Time.deltaTime;

            if (Vector3.Distance(current, target) < 0.05f)
            {
                index++;
            }

            // 👉 TÍNH HƯỚNG MƯỢT
            Vector3 lookDir;
            if (index < path.Count - 1)
                lookDir = (path[index + 1] - current).normalized;
            else
                lookDir = moveDir;

            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                6f * Time.deltaTime
            );
        }

    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Win"))
        {
            PanelWin.FadeIn();
            Timer.SetActive(false);
            Move.SetActive(false);
            TimerSpeed.SetActive(false);
            Option.SetActive(false);
            Run.SetActive(false);

            int move = MoveUI.Instance.GetMoveCount();
            leaderboard.ShowLeaderboard(move);
            SaveData.SavePlayer("YOU", move);

            Time.timeScale = 1f;
        }
    }
}
