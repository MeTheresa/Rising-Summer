using SupanthaPaul;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player1;
    public PlayerController player2;

    void Start()
    {
        PlayerInput input1 = new PlayerInput();
        input1.moveLeftKey = KeyCode.A;
        input1.moveRightKey = KeyCode.D;
        input1.jumpKey = KeyCode.W;
        input1.punchKey = KeyCode.F;

        PlayerInput input2 = new PlayerInput();
        input2.moveLeftKey = KeyCode.LeftArrow;
        input2.moveRightKey = KeyCode.RightArrow;
        input2.jumpKey = KeyCode.UpArrow;
        input2.punchKey = KeyCode.RightControl;

        player1.playerInput = input1;
        player2.playerInput = input2;
    }
}
