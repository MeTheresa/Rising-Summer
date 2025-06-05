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

    private float _fixedPlayerSpeed = 5f;
    private float _fixedBaseKnockbackForce = 8f;
    private float _fixedBaseUpwardKnockbackForce = 7f;
    private float _fixedBaseKnockbackDuration = 0.35f;
    private float _fixedPunchRange = 1f;
    private float _fixedMass = 1f;
    private Vector3 _fixedP1Size; 
    private Vector3 _fixedP2Size;
    void Awake()
    {
        Application.targetFrameRate = 120;
        cam = Camera.main;

        PlayerInput input1 = new PlayerInput();
        PlayerInput input2 = new PlayerInput();

        player1.playerInput = input1;
        player2.playerInput = input2;

        #region Player stats

        _fixedP1Size = Vector3.one;
        switch (MainMenuScript.ClassP1)
        {
            case 0: //Normal size
                {
                    player1.transform.localScale = _fixedP1Size;
                    player1.fixedSpeed = _fixedPlayerSpeed;
                    player1.BaseKnockbackForce = _fixedBaseKnockbackForce;
                    player1.BaseUpwardKnockbackForce = _fixedBaseUpwardKnockbackForce;
                    player1.BaseKnockbackDuration = _fixedBaseKnockbackDuration;
                    player1.PunchRange = _fixedPunchRange;
                    player1.RBPlayer.mass = _fixedMass;
                    break;
                }
            case 1: //Bigger size
                {
                    player1.transform.localScale = new Vector3(_fixedP1Size.x * 1.2f, _fixedP1Size.y*1.2f, 1);
                    player1.fixedSpeed = _fixedPlayerSpeed * 0.8f;
                    player1.BaseKnockbackForce = _fixedBaseKnockbackForce * 1.2f;
                    player1.BaseUpwardKnockbackForce = _fixedBaseUpwardKnockbackForce * 1.2f;
                    player1.BaseKnockbackDuration = _fixedBaseKnockbackDuration * 1.2f;
                    player1.PunchRange = _fixedPunchRange;
                    player1.RBPlayer.mass = _fixedMass * 2;
                    break;
                }
            case 2: //Smaller size
                {
                    player1.transform.localScale = new Vector3(_fixedP1Size.x * 0.8f, _fixedP1Size.y*0.8f, 1);
                    player1.fixedSpeed = _fixedPlayerSpeed * 1.2f;
                    player1.BaseKnockbackForce = _fixedBaseKnockbackForce * 0.8f;
                    player1.BaseUpwardKnockbackForce = _fixedBaseUpwardKnockbackForce * 0.8f;
                    player1.BaseKnockbackDuration = _fixedBaseKnockbackDuration * 0.8f;
                    player1.PunchRange = _fixedPunchRange * 0.8f;
                    player1.RBPlayer.mass = _fixedMass * 0.8f ;
                    break;
                }
        }
        player1.Start();
        Debug.Log(MainMenuScript.ClassP1.ToString());
        _fixedP2Size = Vector3.one;
        switch (MainMenuScript.ClassP2)
        {
            case 0: //Normal size
                {
                    player2.transform.localScale = _fixedP2Size;
                    player2.fixedSpeed = _fixedPlayerSpeed;
                    player2.BaseKnockbackForce = _fixedBaseKnockbackForce;
                    player2.BaseUpwardKnockbackForce = _fixedBaseUpwardKnockbackForce;
                    player2.BaseKnockbackDuration = _fixedBaseKnockbackDuration;
                    player2.PunchRange = _fixedPunchRange;
                    player2.RBPlayer.mass = _fixedMass;
                    break;
                }
            case 1: //Bigger size
                {
                    player2.transform.localScale = new Vector3(_fixedP2Size.x * 1.2f, _fixedP2Size.y * 1.2f, 1);
                    player2.fixedSpeed = _fixedPlayerSpeed * 0.8f;
                    player2.BaseKnockbackForce = _fixedBaseKnockbackForce * 1.2f;
                    player2.BaseUpwardKnockbackForce = _fixedBaseUpwardKnockbackForce * 1.2f;
                    player2.BaseKnockbackDuration = _fixedBaseKnockbackDuration * 1.2f;
                    player2.PunchRange = _fixedPunchRange;
                    player2.RBPlayer.mass = _fixedMass * 2;
                    break;
                }
            case 2: //Smaller size
                {
                    player2.transform.localScale = new Vector3(_fixedP2Size.x * 0.8f, _fixedP2Size.y * 0.8f, 1);
                    player2.fixedSpeed = _fixedPlayerSpeed * 1.2f;
                    player2.BaseKnockbackForce = _fixedBaseKnockbackForce * 0.8f;
                    player2.BaseUpwardKnockbackForce = _fixedBaseUpwardKnockbackForce * 0.8f;
                    player2.BaseKnockbackDuration = _fixedBaseKnockbackDuration * 0.8f;
                    player2.PunchRange = _fixedPunchRange * 0.8f;
                    player2.RBPlayer.mass = _fixedMass * 0.8f;
                    break;
                }
        }
        player2.Start();
        Debug.Log(MainMenuScript.ClassP2.ToString());
        #endregion
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
        if (player1.PowerUpTimer > 0)
        {
            _p1PowerUpSlider.gameObject.SetActive(true);
            switch (player1.WhichPowerUp)
            {
                case 1: //speed
                    {
                        _p1Fill.color = player1.PowerupSpeedColor;
                        _p1Image.color = player1.PowerupSpeedColor;
                        break;
                    }
                case 2: //punch
                    {
                        _p1Fill.color = player1.PowerUpPunchColor;
                        _p1Image.color = player1.PowerUpPunchColor;
                        break;
                    }
            }
            _p1PowerUpSlider.value = player1.PowerUpTimer / player1.PowerUpTime;
        }
        if (player1.PowerUpTimer <= 0)
        {
            _p1PowerUpSlider.gameObject.SetActive(false);
            _p1Image.color = player1.SpriteColor;
        }
        #endregion
        #region Player 2
        if (player2.PowerUpTimer > 0)
        {
            _p2PowerUpSlider.gameObject.SetActive(true);
            switch (player2.WhichPowerUp)
            {
                case 1: //speed
                    {
                        _p2Fill.color = player2.PowerupSpeedColor;
                        _p2Image.color = player2.PowerupSpeedColor;
                        break;
                    }
                case 2: //punch
                    {
                        _p2Fill.color = player2.PowerUpPunchColor;
                        _p2Image.color = player2.PowerUpPunchColor;
                        break;
                    }
            }
            _p2PowerUpSlider.value = player2.PowerUpTimer / player2.PowerUpTime;
        }
        if (player2.PowerUpTimer <= 0)
        {
            _p2PowerUpSlider.gameObject.SetActive(false);
            _p2Image.color = player2.SpriteColor;
        }
        #endregion
        #endregion
    }
}
