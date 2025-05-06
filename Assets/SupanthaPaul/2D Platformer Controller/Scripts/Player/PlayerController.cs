using UnityEngine;

namespace SupanthaPaul
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerInput playerInput;

        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private float upwardKnockbackForce = 7f; // Slightly increased for smoother arc
        [SerializeField] private float punchRange = 1f;

        private Rigidbody2D m_rb;
        private bool m_facingRight = true;
        public bool isGrounded;
        private float moveInput;

        private float knockbackTimer = 0f;
        [SerializeField] private float knockbackDuration = 0.2f;

        void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

            if (knockbackTimer <= 0f)
            {
                m_rb.linearVelocity = new Vector2(moveInput * speed, m_rb.linearVelocity.y);
            }
            else
            {
                knockbackTimer -= Time.fixedDeltaTime;
            }

            if (!m_facingRight && moveInput > 0f)
                Flip();
            else if (m_facingRight && moveInput < 0f)
                Flip();
        }

        void Update()
        {
            if (playerInput != null)
                moveInput = playerInput.HorizontalRaw();

            if (playerInput != null && playerInput.Jump() && isGrounded)
            {
                m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x, jumpForce);
            }

            if (playerInput != null && playerInput.Punch())
            {
                Punch();
            }
        }

        void Punch()
        {
            Vector2 punchPosition = (m_facingRight ? Vector2.right : Vector2.left) * punchRange + (Vector2)transform.position;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(punchPosition, 0.5f, LayerMask.GetMask("Player"));

            foreach (var hit in hitEnemies)
            {
                if (hit && hit.gameObject != gameObject)
                {
                    Rigidbody2D otherRb = hit.GetComponent<Rigidbody2D>();
                    if (otherRb)
                    {
                        otherRb.linearVelocity = Vector2.zero;

                        Vector2 force = new Vector2(
                            m_facingRight ? knockbackForce : -knockbackForce,
                            upwardKnockbackForce
                        );

                        otherRb.AddForce(force, ForceMode2D.Impulse);

                        if (hit.TryGetComponent<PlayerController>(out var otherController))
                        {
                            otherController.knockbackTimer = otherController.knockbackDuration;
                        }
                    }
                }
            }
        }

        void Flip()
        {
            m_facingRight = !m_facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (groundCheck != null)
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
