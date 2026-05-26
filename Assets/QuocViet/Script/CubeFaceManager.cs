using UnityEngine;

public class CubeFaceManager : MonoBehaviour
{
    // ID scene của từng mặt
    public int up = 0;
    public int down = 1;
    public int right = 2;
    public int left = 3;
    public int front = 4;
    public int back = 5;

    // ====== GỌI HÀM NÀY SAU MỖI LẦN LĂN ======

    public void RollForward()
    {
        int temp = up;
        up = back;
        back = down;
        down = front;
        front = temp;
    }

    public void RollBack()
    {
        int temp = up;
        up = front;
        front = down;
        down = back;
        back = temp;
    }

    public void RollRight()
    {
        int temp = up;
        up = left;
        left = down;
        down = right;
        right = temp;
    }

    public void RollLeft()
    {
        int temp = up;
        up = right;
        right = down;
        down = left;
        left = temp;
    }

    public int GetTopFaceSceneIndex()
    {
        return up;
    }
}
