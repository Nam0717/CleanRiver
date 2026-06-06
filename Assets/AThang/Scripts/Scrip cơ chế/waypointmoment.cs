using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public List<Transform> waypoints;
    public float speed = 5f;
    public float reachDistance = 0.2f;

    private int currentPoint = 0;

    private void Start()
    {
        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            Transform target = waypoints[currentPoint];

            while (Vector3.Distance(transform.position, target.position) > reachDistance)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target.position,
                    speed * Time.deltaTime
                );

                transform.forward = Vector3.Lerp(
                    transform.forward,
                    (target.position - transform.position).normalized,
                    5f * Time.deltaTime
                );

                yield return null;
            }

            currentPoint++;

            if (currentPoint >= waypoints.Count)
                currentPoint = 0;
        }
    }
}