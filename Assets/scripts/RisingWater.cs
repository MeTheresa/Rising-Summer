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
    [SerializeField] private bool isGamePaused = false;

    private Collider2D waterCollider;
    [SerializeField] private TMP_Text _timerText;
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
            if (Input.GetKeyDown(KeyCode.Space) || (Gamepad.all.Count > 0 && Input.GetKeyDown(KeyCode.JoystickButton1)))
            {
                ResumeGame();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1f;
                isGamePaused = false;
                SceneManager.LoadScene(0);
            }
        }
        if (!startMoving)
        {
            timer += Time.deltaTime;
            _timerText.text = $"Time before water rises!\n{Mathf.FloorToInt(5-timer)}";
            if (timer >= delayBeforeStart)
                startMoving = true;
        }
        else
        {
            _timerText.gameObject.SetActive(false);
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
        _timerText.gameObject.SetActive(true);
    }
}
