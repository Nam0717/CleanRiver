using UnityEngine;

public class GameController : MonoBehaviour
{
    public PathSolver solver;
    public TrainMover train;
    public FadePanel Lose;

   
      
    
    public void OnGo()
    {
        if (solver.Solve())
        {
            train.StartMoving(solver.finalPath);

            if (!solver.isCompletePath)
            {
                Debug.Log("⚠️ Path sai – tàu sẽ dừng giữa đường");
                Lose.FadeIn();

                // sau này bạn có thể:
                // - rung tàu
                // - đổi màu rail
                // - phát âm thanh sai
            }
        }
    }

}
