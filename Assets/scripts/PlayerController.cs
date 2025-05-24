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
        [SerializeField] private float fixedSpeed = 5f;
        [SerializeField] private float speed = 0f;

        [SerializeField] private float jumpForce = 16f;
        [SerializeField] private bool hasJumped;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask whatIsGround;

        [Header("Punch Settings")]
        public bool isPunching;
        public bool punchAnimation;
        [SerializeField] private float BaseKnockbackForce = 8f;
        [SerializeField] private float knockbackForce = 0f;
        [SerializeField] private float baseUpwardKnockbackForce = 7f;
        [SerializeField] private float upwardKnockbackForce = 0f;
        [SerializeField] private float punchRange = 1f;
        [SerializeField] private float punchCooldown = 1.25f;
        [SerializeField] private float punchAnimationCooldown = 0.25f;
        [SerializeField] private float punchAnimationTimer = 0f;
        [SerializeField] private float punchTimer = 0f;

        [Header("Knockback Settings")]
        [SerializeField] private float BaseKnockbackDuration = 0.35f;
        [SerializeField] private float knockbackDuration = 0f;
        private float knockbackTimer = 0f;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource punchSoundSource;
        [SerializeField] private AudioSource hitSoundSource;
        [SerializeField] private AudioSource runningSoundSource;
        [SerializeField] private AudioSource jumpSoundSource; // New jump sound source

        [Header("Power Ups")]
        public int whichPowerUp = 0;
        public float powerUpTime = 5f;
        public float powerUpTimer = 0f;
        [SerializeField] private float speedPowerUpExtraSpeed = 5f;
        [SerializeField] private float powerUpKnockbackForce = 4f;
        [SerializeField] private float powerUpUpwardKnockbackForce = 2f;
        [SerializeField] private float powerUpKnockbackDuration = 0.15f;

        private Rigidbody2D rb;

        [Header("Other")]
        public Color _spriteColor = Color.white;
        public Color _powerupSpeedColor = Color.blue;
        public Color _powerUpPunchColor = Color.red;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Slider _punchCooldownSlider; 
        private float spriteBlinkingTimer = 0.0f;
        private float spriteBlinkingMiniDuration = 0.1f;
        private float spriteBlinkingTotalTimer = 0.0f;
        private float spriteBlinkingTotalDuration = 0.5f;
        public bool startBlinking = false;
        public SpriteRenderer _spriteRenderer;
        void Start()
        {
            speed = fixedSpeed;
            knockbackForce = BaseKnockbackForce;
            upwardKnockbackForce = baseUpwardKnockbackForce;
            knockbackDuration = BaseKnockbackDuration;
            rb = GetComponent<Rigidbody2D>();

            _spriteRenderer.color = _spriteColor;
            _punchCooldownSlider.gameObject.SetActive(false);
        }
        void Update()
        {
            Movement();
            Punching();
            PowerUps();
            JumpingPhysics();
            HandleRunningSound();
            if (startBlinking) SpriteBlinkingEffect();
            _canvas.transform.position = this.transform.position;
        }
        void FixedUpdate()
        {
            if (knockbackTimer <= 0f)
            {
                rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
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
            isGrounded = Physics2D.Linecast(new Vector3(groundCheck.position.x - _spriteRenderer.size.x / 4, groundCheck.position.y, 0), new Vector3(groundCheck.position.x + _spriteRenderer.size.x / 4, groundCheck.position.y), whatIsGround);

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
                isPunching = false;
                _punchCooldownSlider.gameObject.SetActive(false);
            }
            if (punchAnimationTimer > 0) punchAnimationTimer -= Time.deltaTime;
            if (punchAnimationTimer <= 0) punchAnimation = false;
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
            if (powerUpTimer > 0)
            {
                powerUpTimer -= Time.deltaTime;
            }
            else
            {
                powerUpTimer = 0;
                switch (whichPowerUp)
                {
                    case 1:
                        {
                            _spriteRenderer.color = _spriteColor;
                            speed = fixedSpeed;
                            whichPowerUp = 0;
                            Debug.Log("PowerUp 1 down" + this.gameObject.name);
                            break;
                        }
                    case 2:
                        {
                            _spriteRenderer.color = _spriteColor;
                            knockbackForce = BaseKnockbackForce;
                            upwardKnockbackForce = baseUpwardKnockbackForce;
                            otherPlayerController.knockbackDuration = BaseKnockbackDuration;
                            whichPowerUp = 0;
                            Debug.Log("PowerUp 2 down" + this.gameObject.name);
                            break;
                        }
                    case 3:
                        {
                            _spriteRenderer.color = _spriteColor;
                            whichPowerUp = 0;
                            Debug.Log("PowerUp 3 down" + this.gameObject.name);
                            break;
                        }
                }
            }
        }
        private void ApplyJumpVelocity()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        private void PlayJumpSound()
        {
            jumpSoundSource.Play();
        }
        void Punch()
        {
            if (!isPunching)
            {
                isPunching = true;
                punchAnimation = true;
                punchTimer = punchCooldown;
                punchAnimationTimer = punchAnimationCooldown;
                Vector2 punchPosition = (facingRight ? Vector2.right : Vector2.left) * punchRange + (Vector2)transform.position;
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(punchPosition, 0.5f, LayerMask.GetMask("Player"));

                bool hitEnemy = false;

                foreach (var hit in hitEnemies)
                {
                    if (hit && hit.gameObject != gameObject)
                    {
                        hitEnemy = true;
                        Rigidbody2D otherRb = hit.GetComponent<Rigidbody2D>();
                        hit.GetComponent<PlayerController>().startBlinking = true;
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
                startBlinking = false;
                spriteBlinkingTotalTimer = 0.0f;
               _spriteRenderer.enabled = true;
                return;
            }

            spriteBlinkingTimer += Time.deltaTime;
            if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
            {
                spriteBlinkingTimer = 0.0f;
                if (_spriteRenderer.enabled == true)
                {
                    _spriteRenderer.enabled = false;  //make changes
                }
                else
                {
                    _spriteRenderer.enabled = true;   //make changes
                }
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (groundCheck != null)
                // Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
                Gizmos.DrawLine(new Vector3(groundCheck.position.x - _spriteRenderer.size.x / 4, groundCheck.position.y, 0), new Vector3(groundCheck.position.x + _spriteRenderer.size.x / 4, groundCheck.position.y));
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
            {
                powerUpTimer = powerUpTime;
                switch (collision.name)
                {
                    case "PowerUp1(Clone)": //speed
                        {
                            whichPowerUp = 1;
                            speed = fixedSpeed + speedPowerUpExtraSpeed;
                            Debug.Log("PowerUp 1 picked up" + this.gameObject.name);
                            knockbackForce = BaseKnockbackForce;
                            upwardKnockbackForce = baseUpwardKnockbackForce;
                            otherPlayerController.knockbackDuration = BaseKnockbackDuration;
                            _spriteRenderer.color = _powerupSpeedColor;
                            break;
                        }
                    case "PowerUp2(Clone)": //punch
                        {
                            whichPowerUp = 2;
                            knockbackForce = BaseKnockbackForce + powerUpKnockbackForce;
                            upwardKnockbackForce = baseUpwardKnockbackForce + powerUpUpwardKnockbackForce;
                            otherPlayerController.knockbackDuration = BaseKnockbackDuration + powerUpKnockbackDuration;
                            Debug.Log("PowerUp 2 picked up" + this.gameObject.name);
                            speed = fixedSpeed;
                            _spriteRenderer.color = _powerUpPunchColor;
                            break;
                        }
                    case "PowerUp3(Clone)":
                        {
                            whichPowerUp = 3;
                            break;
                        }
                }
                Destroy(collision.gameObject);
            }
        }
    }
}
