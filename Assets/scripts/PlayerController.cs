using UnityEngine;

namespace SupanthaPaul
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerInput playerInput;

        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private float _knockbackForce = 10f;
        [SerializeField] private float _upwardKnockbackForce = 7f; // Slightly increased for smoother arc
        [SerializeField] private float _punchRange = 1f;

        private Rigidbody2D _m_rb;
        private bool _m_facingRight = true;
        public bool _isGrounded;
        public bool _isPunching;
        [SerializeField] private float _moveInput;

        private float knockbackTimer = 0f;
        [SerializeField] private float _knockbackDuration = 0.2f;

        void Start()
        {
            _m_rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _whatIsGround);

            if (knockbackTimer <= 0f)
            {
                _m_rb.linearVelocity = new Vector2(_moveInput * _speed, _m_rb.linearVelocity.y);
            }
            else
            {
                knockbackTimer -= Time.fixedDeltaTime;
            }

            if (!_m_facingRight && _moveInput > 0f)
                Flip();
            else if (_m_facingRight && _moveInput < 0f)
                Flip();
        }

        void Update()
        {
            if (playerInput != null)
                _moveInput = playerInput.HorizontalRaw();
            else Debug.Log("Did not find PlayerInput");

            if (playerInput != null && playerInput.Jump() && _isGrounded)
            {
                _m_rb.linearVelocity = new Vector2(_m_rb.linearVelocity.x, _jumpForce);
            }

            if (playerInput != null && playerInput.Punch())
            {
                Punch();
            }
        }

    void Punch()
    {
        _isPunching = true;
        Invoke(nameof(ResetPunch), 0.2f); // Reset after 0.2 seconds

        Vector2 punchPosition = (_m_facingRight ? Vector2.right : Vector2.left) * _punchRange + (Vector2)transform.position;
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
                        _m_facingRight ? _knockbackForce : -_knockbackForce,
                        _upwardKnockbackForce
                    );

                    otherRb.AddForce(force, ForceMode2D.Impulse);

                    if (hit.TryGetComponent<PlayerController>(out var otherController))
                    {
                        otherController.knockbackTimer = otherController._knockbackDuration;
                    }
                }
            }
        }
    }

        void ResetPunch()
        {
            _isPunching = false;
        }

        void Flip()
        {
            _m_facingRight = !_m_facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (_groundCheck != null)
                Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
        }
    }
}
