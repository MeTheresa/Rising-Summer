using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RisingWater : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1.25f;
    [SerializeField] private float delayBeforeStart = 5f;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    public GameObject GameOverCanvas;

    public TextMeshProUGUI player1Wins;
    public TextMeshProUGUI player2Wins;
    public TextMeshProUGUI winnerText;
    

    private GameObject PlayerWhoDied;
    private float timer = 0f;
    private bool startMoving = false;
    private bool isGamePaused = false;

    private Collider2D waterCollider;

    void Start()
    {
        waterCollider = GetComponent<Collider2D>();
        if (!waterCollider.isTrigger)
            waterCollider.isTrigger = true;

        player1Wins.text = "Player 1 Wins: " + ScoreManager.Instance.player1Wins;
        player2Wins.text = "Player 2 Wins: " + ScoreManager.Instance.player2Wins;

        GameOverCanvas.SetActive(false);
    }

    void Update()
    {
        if (isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Space) || (Gamepad.all.Count > 0 && Gamepad.all[0].buttonWest.wasPressedThisFrame))
            {
                ResumeGame();
            }
        }
        if (!startMoving)
        {
            timer += Time.deltaTime;
            if (timer >= delayBeforeStart)
                startMoving = true;
        }
        else
        {
            transform.position += Vector3.up * scrollSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player1)
        {
            PlayerWhoDied = player1;
            KillPlayers();
        }
        if(other.gameObject == player2)
        {
            PlayerWhoDied = player2;
            KillPlayers();
        }
    }

    void KillPlayers()
    {
        Debug.Log($"Game Over! {PlayerWhoDied.name} lost!");

        string winner = (PlayerWhoDied == player1) ? "Player2" : "Player1";
        ScoreManager.Instance.AddWin(winner);

        //SceneManager.LoadScene("SceneGameOver");
        GameOverCanvas.SetActive(true);
        winnerText.text = $"{winner} Wins!";
        Time.timeScale = 0f;
        isGamePaused = true;
    }
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
