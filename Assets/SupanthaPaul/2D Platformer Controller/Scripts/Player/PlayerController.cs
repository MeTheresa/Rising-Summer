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
        [SerializeField] private float knockbackForce = 10f; // The force applied on the enemy when punched
        [SerializeField] private float punchRange = 1f; // Range of the punch

        [SerializeField] private Vector2 knockbackDirection;
        private Rigidbody2D m_rb;
        [SerializeField] private bool m_facingRight = true;
        public bool isGrounded;
        private float moveInput;

        // For detecting other players in punch range
        private Collider2D[] hitEnemies;

        void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            // Check if grounded
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

            // Move the player
            m_rb.linearVelocity = new Vector2(moveInput * speed, m_rb.linearVelocity.y);

            // Flip sprite if needed
            if (!m_facingRight && moveInput > 0f)
                Flip();
            else if (m_facingRight && moveInput < 0f)
                Flip();
        }

        void Update()
        {
            // Get horizontal input
            if (playerInput != null)
                moveInput = playerInput.HorizontalRaw();

            // Jump
            if (playerInput != null && playerInput.Jump() && isGrounded)
            {
                m_rb.linearVelocity = new Vector2(m_rb.linearVelocity.x, jumpForce);
            }

            // Check for punch input
            if (playerInput != null && playerInput.Punch()) // "F" key in the input
            {
                Punch();
            }
        }

        // Function for punching
        void Punch()
        {
            // Determine the position to check for punch collision based on the player's facing direction
            Vector2 punchPosition = (m_facingRight) ? (Vector2)transform.position + Vector2.right * punchRange : (Vector2)transform.position + Vector2.left * punchRange;

            // Cast a hitbox in the direction the player is facing (detecting only other players)
            hitEnemies = Physics2D.OverlapCircleAll(punchPosition, 0.5f, LayerMask.GetMask("Player")); // We will knockback players in range

            foreach (var hit in hitEnemies)
            {
                if (hit != null && hit.gameObject != this.gameObject) // Avoid hitting itself
                {
                    // Debugging which enemy is being hit
                    Debug.Log("Punch hit: " + hit.gameObject.name);

                    // Apply a more reasonable forward and upward force
                    float forwardForce = knockbackForce;  // Horizontal force, adjust to your liking
                    float upwardForce = 10f;  // Vertical force, adjust to your liking

                    // Adjust the direction of the knockback based on which way the player is facing
                    knockbackDirection = m_facingRight ? Vector2.right : Vector2.left;
                    knockbackDirection.y = Vector2.up.y;

                    // Apply the knockback force to the other player's Rigidbody2D
                    Rigidbody2D otherRb = hit.GetComponent<Rigidbody2D>();
                    knockbackDirection.x *= forwardForce;
                    knockbackDirection.y = upwardForce;
                    knockbackDirection *= knockbackForce;

                    if (otherRb != null)
                    {
                        // Apply the combined forward and upward force
                        otherRb.AddForce(knockbackDirection, ForceMode2D.Force);

                        // Log the force applied
                        Debug.Log("Applying knockback with force: " + (knockbackDirection * forwardForce + Vector2.up * upwardForce));
                    }
                }
            }
        }




        // Function to flip the player based on movement direction
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
