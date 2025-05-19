using UnityEngine;
using UnityEngine.SceneManagement;

public class RisingWater : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1.25f;
    [SerializeField] private float delayBeforeStart = 5f;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    private GameObject PlayerWhoDied;
    private float timer = 0f;
    private bool startMoving = false;

    private Collider2D waterCollider;

    void Start()
    {
        waterCollider = GetComponent<Collider2D>();
        if (!waterCollider.isTrigger)
            waterCollider.isTrigger = true;
    }

    void Update()
    {
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

        SceneManager.LoadScene("SceneGameOver");
    }
}
