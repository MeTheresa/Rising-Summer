using System;
using Unity.VisualScripting;
using UnityEngine;

namespace SupanthaPaul
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerInput playerInput;
        public PlayerController otherPlayerController;

        [Header("Movement Settings")]
        [SerializeField] private float fixedSpeed = 5f;
        [SerializeField] private float speed = 0f;

        [SerializeField] private float jumpForce = 16f;
        [SerializeField] private bool hasJumped;
        [SerializeField] private bool hasDoubleJumped;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask whatIsGround;

        [Header("Punch Settings")]
        [SerializeField] private float BaseKnockbackForce = 8f;
        [SerializeField] private float knockbackForce = 0f;
        [SerializeField] private float baseUpwardKnockbackForce = 7f;
        [SerializeField] private float upwardKnockbackForce = 0f;
        [SerializeField] private float punchRange = 1f;

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
        [SerializeField] private int whichPowerUp = 0;
        [SerializeField] private float powerUpTime = 5f;
        [SerializeField] private float powerUpTimer = 0f;
        [SerializeField] private float speedPowerUp = 5f;
        [SerializeField] private float powerUpKnockbackForce= 4f;
        [SerializeField] private float powerUpUpwardKnockbackForce = 2f;
        [SerializeField] private float powerUpKnockbackDuration = 0.15f;



        private Rigidbody2D rb;
        private bool facingRight = true;
        public bool isGrounded;
        public bool isPunching;
        private float moveInput;

        void Start()
        {
            speed = fixedSpeed;
            knockbackForce = BaseKnockbackForce;
            upwardKnockbackForce = baseUpwardKnockbackForce;
            knockbackDuration = BaseKnockbackDuration;
            rb = GetComponent<Rigidbody2D>();
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

        private void ApplyJumpVelocity()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        private void PlayJumpSound()
        {
            jumpSoundSource.Play();
        }

        void Update()
        {
            if (isGrounded && hasJumped || isGrounded && hasDoubleJumped)
            {
                hasJumped = false;
                hasDoubleJumped = false;
            }
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            
            if (playerInput != null)
                moveInput = playerInput.HorizontalRaw();

            if (playerInput.Punch())
            {
                Punch();
            }
            
            HandleRunningSound();

            if (powerUpTimer > 0) powerUpTimer -= Time.deltaTime;
            else
            {
                powerUpTimer = 0;
                switch (whichPowerUp)
                {
                    case 1:
                        {
                            speed = fixedSpeed;
                            whichPowerUp = 0;
                            Debug.Log("PowerUp 1 down" + this.gameObject.name);
                            break;
                        }
                    case 2:
                        {
                            knockbackForce = BaseKnockbackForce;
                            upwardKnockbackForce = baseUpwardKnockbackForce;
                            otherPlayerController.knockbackDuration = BaseKnockbackDuration;
                            whichPowerUp = 0;
                            Debug.Log("PowerUp 2 down" + this.gameObject.name);
                            break;
                        }
                    case 3:
                        {
                            whichPowerUp = 0;
                            Debug.Log("PowerUp 3 down" + this.gameObject.name);
                            break;
                        }
                }
                
            }
        }
        void LateUpdate()
        {
            if (hasJumped && playerInput.Jump() && hasDoubleJumped == false)
            {
                hasDoubleJumped = true;
                ApplyJumpVelocity();

                // Play jump sound
                if (jumpSoundSource != null)
                {
                    PlayJumpSound();
                }
            }
            if (playerInput.Jump() && isGrounded)
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

        void Punch()
        {
            isPunching = true;
            Invoke(nameof(ResetPunch), 0.2f); // Reset after 0.2 seconds

            Vector2 punchPosition = (facingRight ? Vector2.right : Vector2.left) * punchRange + (Vector2)transform.position;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(punchPosition, 0.5f, LayerMask.GetMask("Player"));

            bool hitEnemy = false;

            foreach (var hit in hitEnemies)
            {
                if (hit && hit.gameObject != gameObject)
                {
                    hitEnemy = true;
                    Rigidbody2D otherRb = hit.GetComponent<Rigidbody2D>();
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

        void ResetPunch()
        {
            isPunching = false;
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (groundCheck != null)
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            powerUpTimer = powerUpTime;
            switch (collision.name) 
            {
                case "PowerUp1(Clone)":
                    {
                        whichPowerUp = 1;
                        speed = fixedSpeed + speedPowerUp;
                            Debug.Log("PowerUp 1 picked up" + this.gameObject.name);
                        knockbackForce = BaseKnockbackForce;
                        upwardKnockbackForce = baseUpwardKnockbackForce;
                        otherPlayerController.knockbackDuration = BaseKnockbackDuration;

                        break;
                    }
                case "PowerUp2(Clone)":
                    {
                        whichPowerUp = 2;
                        knockbackForce = BaseKnockbackForce + powerUpKnockbackForce;
                        upwardKnockbackForce = baseUpwardKnockbackForce + powerUpUpwardKnockbackForce;
                        otherPlayerController.knockbackDuration = BaseKnockbackDuration + powerUpKnockbackDuration;
                        Debug.Log("PowerUp 2 picked up" + this.gameObject.name);
                        speed = fixedSpeed;
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
