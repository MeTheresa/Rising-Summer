using SupanthaPaul;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player1;
    public PlayerController player2;

    [SerializeField] private Camera cam;

    [SerializeField] private float powerUpCooldown = 2f;
    [SerializeField] private float powerUpCdTimer = 0f;
    [SerializeField] private Vector2 randomPowerUpPos;
    [SerializeField] private GameObject powerUp1;
    [SerializeField] private GameObject powerUp2;
    [SerializeField] private GameObject powerUp3;

    void Start()
    {
        Application.targetFrameRate = 60;
        cam = Camera.main;

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
    void Update()
    {
        powerUpCdTimer -= Time.deltaTime;
        if (powerUpCdTimer <= 0f)
        {
            powerUpCdTimer = powerUpCooldown;
            randomPowerUpPos = new Vector2(Random.Range(cam.transform.position.x - cam.orthographicSize* cam.aspect, cam.transform.position.x + cam.orthographicSize * cam.aspect),
                                           Random.Range(cam.transform.position.y - cam.orthographicSize, cam.transform.position.y + cam.orthographicSize) );
            for (int i = 0; i == 1; i++)
            {
                randomPowerUpPos = new Vector2(Random.Range(cam.transform.position.x - cam.orthographicSize * cam.aspect, cam.transform.position.x + cam.orthographicSize * cam.aspect),
                                           Random.Range(cam.transform.position.y - cam.orthographicSize, cam.transform.position.y + cam.orthographicSize));
                if (Physics2D.BoxCast(randomPowerUpPos, new Vector2(0.6f, 0.6f), 0, Vector2.zero))
                {
                    i--;
                }
            }
            float randomNumber = Random.Range(1, 3);
            switch (randomNumber)
            {
                case 1:
                    {
                        Instantiate(powerUp1, new Vector3(randomPowerUpPos.x, randomPowerUpPos.y, 0), Quaternion.identity);
                        Debug.Log("1st powerup spawned");
                        break;
                    }
                case 2:
                    {
                        Instantiate(powerUp2, new Vector3(randomPowerUpPos.x, randomPowerUpPos.y, 0), Quaternion.identity);
                        Debug.Log("2nd powerup spawned");
                        break;
                    }
                default:
                    {
                        Debug.Log("3rd powerup spawned");
                        break;
                    }
            }
            
        }
    }
}
