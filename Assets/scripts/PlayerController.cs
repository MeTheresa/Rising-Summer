using UnityEngine;

namespace SupanthaPaul
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerInput playerInput;

        [Header("Movement Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 10f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask whatIsGround;

        [Header("Punch Settings")]
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private float upwardKnockbackForce = 7f;
        [SerializeField] private float punchRange = 1f;

        [Header("Knockback Settings")]
        [SerializeField] private float knockbackDuration = 0.2f;
        private float knockbackTimer = 0f;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource punchSoundSource;
        [SerializeField] private AudioSource hitSoundSource;
        [SerializeField] private AudioSource runningSoundSource;
        [SerializeField] private AudioSource jumpSoundSource; // New jump sound source

        private Rigidbody2D rb;
        private bool facingRight = true;
        public bool isGrounded;
        public bool isPunching;
        private float moveInput;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

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

        void Update()
        {
            if (playerInput != null)
                moveInput = playerInput.HorizontalRaw();
            else Debug.Log("Did not find PlayerInput");

            if (playerInput != null && playerInput.Jump() && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                // Play jump sound
                if (jumpSoundSource != null)
                {
                    jumpSoundSource.Play();
                }
            }

            if (playerInput != null && playerInput.Punch())
            {
                Punch();
            }

            HandleRunningSound();
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
    }
}
