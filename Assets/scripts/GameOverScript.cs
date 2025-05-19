using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public TextMeshProUGUI player1Wins;
    public TextMeshProUGUI player2Wins;
    public TextMeshProUGUI winnerText;

    void Start()
    {
        player1Wins.text = "Player 1 Wins: " + ScoreManager.Instance.player1Wins;
        player2Wins.text = "Player 2 Wins: " + ScoreManager.Instance.player2Wins;

        string winner = ScoreManager.Instance.lastWinner;
        winnerText.text = winner + " Wins!";
    }
    public void Retry()
    {
        SceneManager.LoadScene("SceneLevel_1");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("SceneMainMenu");
    }

}
