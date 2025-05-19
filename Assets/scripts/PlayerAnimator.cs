using UnityEngine;

namespace SupanthaPaul
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Rigidbody2D m_rb;
        private PlayerController m_controller;
        private Animator m_anim;

        private static readonly int _move = Animator.StringToHash("Move");
        private static readonly int _isJumping = Animator.StringToHash("IsJumping");
        private static readonly int _melee = Animator.StringToHash("Melee");

        private void Start()
        {
            m_anim = GetComponentInChildren<Animator>();
            m_controller = GetComponent<PlayerController>();
            m_rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Idle & Running animation (absolute X velocity)
            m_anim.SetFloat(_move, Mathf.Abs(m_rb.linearVelocity.x));

            // Jump animation (if not grounded)
            m_anim.SetBool(_isJumping, !m_controller.isGrounded);

            m_anim.SetBool(_melee, m_controller.punchAnimation);

        }
    }
}
