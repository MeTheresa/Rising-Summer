using SupanthaPaul;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("UI")]
    [SerializeField] private Slider _p1PowerUpSlider;
    [SerializeField] private Slider _p2PowerUpSlider;
    [SerializeField] private Image _p1Image;
    [SerializeField] private Image _p2Image;
    [SerializeField] private Image _p1Fill;
    [SerializeField] private Image _p2Fill;
    void Awake()
    {
        Application.targetFrameRate = 120;
        cam = Camera.main;

        PlayerInput input1 = new PlayerInput();
        PlayerInput input2 = new PlayerInput();

        player1.playerInput = input1;
        player2.playerInput = input2;
    }
    void Update()
    {
        #region Spawning Power ups
        if (!gameObject.activeInHierarchy || cam == null) return;
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
        #endregion
        #region Player power up UI
        #region Player 1
        if (player1.powerUpTimer > 0)
        {
            _p1PowerUpSlider.gameObject.SetActive(true);
            switch (player1.whichPowerUp)
            {
                case 1: //speed
                    {
                        _p1Fill.color = player1._powerupSpeedColor;
                        _p1Image.color = player1._powerupSpeedColor;
                        break;
                    }
                case 2: //punch
                    {
                        _p1Fill.color = player1._powerUpPunchColor;
                        _p1Image.color = player1._powerUpPunchColor;
                        break;
                    }
            }
            _p1PowerUpSlider.value = player1.powerUpTimer / player1.powerUpTime;
        }
        if (player1.powerUpTimer <= 0)
        {
            _p1PowerUpSlider.gameObject.SetActive(false);
            _p1Image.color = player1._spriteColor;
        }
        #endregion
        #region Player 2
        if (player2.powerUpTimer > 0)
        {
            _p2PowerUpSlider.gameObject.SetActive(true);
            switch (player2.whichPowerUp)
            {
                case 1: //speed
                    {
                        _p2Fill.color = player2._powerupSpeedColor;
                        _p2Image.color = player2._powerupSpeedColor;
                        break;
                    }
                case 2: //punch
                    {
                        _p2Fill.color = player2._powerUpPunchColor;
                        _p2Image.color = player2._powerUpPunchColor;
                        break;
                    }
            }
            _p2PowerUpSlider.value = player2.powerUpTimer / player2.powerUpTime;
        }
        if (player2.powerUpTimer <= 0)
        {
            _p2PowerUpSlider.gameObject.SetActive(false);
            _p2Image.color = player2._spriteColor;
        }
        #endregion
        #endregion
    }
}
