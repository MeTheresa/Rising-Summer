using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SupanthaPaul
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerInput playerInput;
        public PlayerController otherPlayerController;

        [Header("Movement Settings")]
        public bool isGrounded;
        private bool facingRight = true;
        private float moveInput;
        public float fixedSpeed = 5f;
        [SerializeField] private float speed = 0f;

        [SerializeField] private float jumpForce = 16f;
        [SerializeField] private bool hasJumped;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask whatIsGround;

        [Header("Punch Settings")]
        public bool IsPunching;
        public bool PunchAnimation;
        public float BaseKnockbackForce = 8f;
        [SerializeField] private float knockbackForce = 0f;
        public float BaseUpwardKnockbackForce = 7f;
        [SerializeField] private float upwardKnockbackForce = 0f;
        public float PunchRange = 1f;
        [SerializeField] private float punchCooldown = 1.25f;
        [SerializeField] private float punchAnimationCooldown = 0.25f;
        [SerializeField] private float punchAnimationTimer = 0f;
        [SerializeField] private float punchTimer = 0f;

        [Header("Knockback Settings")]
        public float BaseKnockbackDuration = 0.35f;
        [SerializeField] private float knockbackDuration = 0f;
        private float knockbackTimer = 0f;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource punchSoundSource;
        [SerializeField] private AudioSource hitSoundSource;
        [SerializeField] private AudioSource runningSoundSource;
        [SerializeField] private AudioSource jumpSoundSource; // New jump sound source

        [Header("Power Ups")]
        public int WhichPowerUp = 0;
        public float PowerUpTime = 5f;
        public float PowerUpTimer = 0f;
        [SerializeField] private float speedPowerUpExtraSpeed = 5f;
        [SerializeField] private float powerUpKnockbackForce = 4f;
        [SerializeField] private float powerUpUpwardKnockbackForce = 2f;
        [SerializeField] private float powerUpKnockbackDuration = 0.15f;

        public Rigidbody2D RBPlayer;

        [Header("Other")]
        public Color SpriteColor = Color.white;
        public Color PowerupSpeedColor = Color.blue;
        public Color PowerUpPunchColor = Color.red;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Slider _punchCooldownSlider; 
        private float spriteBlinkingTimer = 0.0f;
        private float spriteBlinkingMiniDuration = 0.1f;
        private float spriteBlinkingTotalTimer = 0.0f;
        private float spriteBlinkingTotalDuration = 0.5f;
        public bool StartBlinking = false;
        public SpriteRenderer PlayerSpriteRenderer;
        public void Start()
        {
            speed = fixedSpeed;
            knockbackForce = BaseKnockbackForce;
            upwardKnockbackForce = BaseUpwardKnockbackForce;
            knockbackDuration = BaseKnockbackDuration;

            PlayerSpriteRenderer.color = SpriteColor;
            _punchCooldownSlider.gameObject.SetActive(false);
        }
        void Update()
        {
            Movement();
            Punching();
            PowerUps();
            JumpingPhysics();
            HandleRunningSound();
            if (StartBlinking) SpriteBlinkingEffect();
            _canvas.transform.position = this.transform.position;
        }
        void FixedUpdate()
        {
            if (knockbackTimer <= 0f)
            {
                RBPlayer.linearVelocity = new Vector2(moveInput * speed, RBPlayer.linearVelocity.y);
            }
            else
            {
                knockbackTimer -= Time.fixedDeltaTime;
            }

            if (!facingRight && moveInput > 0f)
                Flip();
            else if (facingRight && moveInput < 0f)
                Flip();
        }
        private void Movement()
        {
            if (isGrounded && hasJumped)
            {
                hasJumped = false;
            }
            isGrounded = Physics2D.Linecast(new Vector3(groundCheck.position.x - PlayerSpriteRenderer.size.x / 4, groundCheck.position.y, 0), new Vector3(groundCheck.position.x + PlayerSpriteRenderer.size.x / 4, groundCheck.position.y), whatIsGround);

            if (this.gameObject.name == "Player1")
            {
                if (playerInput != null)
                {
                    moveInput = playerInput.HorizontalRawP1();
                }
            }
            else if (this.gameObject.name == "Player2")
            {
                if (playerInput != null)
                {
                    moveInput = playerInput.HorizontalRawP2();
                }
            }
        }
        private void Punching()
        {
            if (this.gameObject.name == "Player1")
            {
                if (playerInput.PunchP1())
                {
                    Punch();
                    punchTimer -= Time.deltaTime;
                    _punchCooldownSlider.gameObject.SetActive(true);

                }
            }
            else if (this.gameObject.name == "Player2")
            {
                if (playerInput.PunchP2())
                {
                    Punch();
                    punchTimer -= Time.deltaTime;
                    _punchCooldownSlider.gameObject.SetActive(true);
                }
            }
            if (punchTimer > 0)
            {
                punchTimer -= Time.deltaTime;
                _punchCooldownSlider.value = 1 - punchTimer/punchCooldown;
            }
            if (punchTimer <= 0)
            {
                IsPunching = false;
                _punchCooldownSlider.gameObject.SetActive(false);
            }
            if (punchAnimationTimer > 0) punchAnimationTimer -= Time.deltaTime;
            if (punchAnimationTimer <= 0) PunchAnimation = false;
        }
        private void JumpingPhysics()
        {
            if (!isGrounded) hasJumped = true;
            if (this.gameObject.name == "Player1")
            {
                if (playerInput.JumpP1() && isGrounded)
                {
                    isGrounded = false;
                    hasJumped = true;
                    ApplyJumpVelocity();

                    // Play jump sound
                    if (jumpSoundSource != null)
                    {
                        PlayJumpSound();
                    }
                }
            }
            if (this.gameObject.name == "Player2")
            {
                if (playerInput.JumpP2() && isGrounded)
                {
                    isGrounded = false;
                    hasJumped = true;
                    ApplyJumpVelocity();

                    // Play jump sound
                    if (jumpSoundSource != null)
                    {
                        PlayJumpSound();
                    }
                }
            }
        }
        private void PowerUps()
        {
            if (PowerUpTimer > 0)
            {
                PowerUpTimer -= Time.deltaTime;
            }
            else
            {
                PowerUpTimer = 0;
                switch (WhichPowerUp)
                {
                    case 1:
                        {
                            PlayerSpriteRenderer.color = SpriteColor;
                            speed = fixedSpeed;
                            WhichPowerUp = 0;
                            Debug.Log("PowerUp 1 down" + this.gameObject.name);
                            break;
                        }
                    case 2:
                        {
                            PlayerSpriteRenderer.color = SpriteColor;
                            knockbackForce = BaseKnockbackForce;
                            upwardKnockbackForce = BaseUpwardKnockbackForce;
                            otherPlayerController.knockbackDuration = BaseKnockbackDuration;
                            WhichPowerUp = 0;
                            Debug.Log("PowerUp 2 down" + this.gameObject.name);
                            break;
                        }
                    case 3:
                        {
                            PlayerSpriteRenderer.color = SpriteColor;
                            WhichPowerUp = 0;
                            Debug.Log("PowerUp 3 down" + this.gameObject.name);
                            break;
                        }
                }
            }
        }
        private void ApplyJumpVelocity()
        {
            RBPlayer.linearVelocity = new Vector2(RBPlayer.linearVelocity.x, jumpForce);
        }
        private void PlayJumpSound()
        {
            jumpSoundSource.Play();
        }
        void Punch()
        {
            if (!IsPunching)
            {
                IsPunching = true;
                PunchAnimation = true;
                punchTimer = punchCooldown;
                punchAnimationTimer = punchAnimationCooldown;
                Vector2 punchPosition = (facingRight ? Vector2.right : Vector2.left) * PunchRange + (Vector2)transform.position;
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(punchPosition, 0.5f, LayerMask.GetMask("Player"));

                bool hitEnemy = false;

                foreach (var hit in hitEnemies)
                {
                    if (hit && hit.gameObject != gameObject)
                    {
                        hitEnemy = true;
                        Rigidbody2D otherRb = hit.GetComponent<Rigidbody2D>();
                        hit.GetComponent<PlayerController>().StartBlinking = true;
                        if (otherRb)
                        {
                            otherRb.linearVelocity = Vector2.zero;

                            Vector2 force = new Vector2(
                                facingRight ? knockbackForce : -knockbackForce,
                                upwardKnockbackForce
                            );

                            otherRb.AddForce(force, ForceMode2D.Impulse);

                            // Play hit sound
                            if (hitSoundSource != null)
                            {
                                hitSoundSource.Play();
                            }

                            if (hit.TryGetComponent<PlayerController>(out var otherController))
                            {
                                otherController.knockbackTimer = otherController.knockbackDuration;
                            }
                        }
                    }
                }
                // Play punch sound only if no enemy was hit
                if (!hitEnemy && punchSoundSource != null)
                {
                    punchSoundSource.Play();
                }
            }
        }
        void Flip()
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        void HandleRunningSound()
        {
            if (Mathf.Abs(moveInput) > 0.1f && isGrounded)
            {
                if (!runningSoundSource.isPlaying)
                {
                    runningSoundSource.Play();
                }
            }
            else
            {
                if (runningSoundSource.isPlaying)
                {
                    runningSoundSource.Stop();
                }
            }
        }
        private void SpriteBlinkingEffect()
        {
            spriteBlinkingTotalTimer += Time.deltaTime;
            if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
            {
                StartBlinking = false;
                spriteBlinkingTotalTimer = 0.0f;
               PlayerSpriteRenderer.enabled = true;
                return;
            }

            spriteBlinkingTimer += Time.deltaTime;
            if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
            {
                spriteBlinkingTimer = 0.0f;
                if (PlayerSpriteRenderer.enabled == true)
                {
                    PlayerSpriteRenderer.enabled = false;  //make changes
                }
                else
                {
                    PlayerSpriteRenderer.enabled = true;   //make changes
                }
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (groundCheck != null)
                // Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
                Gizmos.DrawLine(new Vector3(groundCheck.position.x - PlayerSpriteRenderer.size.x / 4, groundCheck.position.y, 0), new Vector3(groundCheck.position.x + PlayerSpriteRenderer.size.x / 4, groundCheck.position.y));
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
            {
                PowerUpTimer = PowerUpTime;
                switch (collision.name)
                {
                    case "PowerUp1(Clone)": //speed
                        {
                            WhichPowerUp = 1;
                            speed = fixedSpeed + speedPowerUpExtraSpeed;
                            Debug.Log("PowerUp 1 picked up" + this.gameObject.name);
                            knockbackForce = BaseKnockbackForce;
                            upwardKnockbackForce = BaseUpwardKnockbackForce;
                            otherPlayerController.knockbackDuration = BaseKnockbackDuration;
                            PlayerSpriteRenderer.color = PowerupSpeedColor;
                            break;
                        }
                    case "PowerUp2(Clone)": //punch
                        {
                            WhichPowerUp = 2;
                            knockbackForce = BaseKnockbackForce + powerUpKnockbackForce;
                            upwardKnockbackForce = BaseUpwardKnockbackForce + powerUpUpwardKnockbackForce;
                            otherPlayerController.knockbackDuration = BaseKnockbackDuration + powerUpKnockbackDuration;
                            Debug.Log("PowerUp 2 picked up" + this.gameObject.name);
                            speed = fixedSpeed;
                            PlayerSpriteRenderer.color = PowerUpPunchColor;
                            break;
                        }
                    case "PowerUp3(Clone)":
                        {
                            WhichPowerUp = 3;
                            break;
                        }
                }
                Destroy(collision.gameObject);
            }
        }
    }
}
