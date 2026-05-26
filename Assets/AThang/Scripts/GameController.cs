using UnityEngine;

public class GameController : MonoBehaviour
{
    public PathSolver solver;
    public TrainMover train;

    public void OnGo()
    {
        if (solver.Solve())
            train.StartMoving(solver.finalPath);
        else
            Debug.Log("❌ Puzzle chưa đúng");
    }
}
