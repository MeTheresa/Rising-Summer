using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int player1Wins = 0;
    public int player2Wins = 0;
    public string lastWinner;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddWin(string playerName)
    {
        if (playerName == "Player1")
        {
            player1Wins++;
        }
        else if (playerName == "Player2")
        {
            player2Wins++;
        }

        lastWinner = playerName;
    }
}
