using UnityEngine;

namespace SupanthaPaul
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Rigidbody2D m_rb;
        private PlayerController m_controller;
        private Animator m_anim;

        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");

        private void Start()
        {
            m_anim = GetComponentInChildren<Animator>();
            m_controller = GetComponent<PlayerController>();
            m_rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Idle & Running animation (absolute X velocity)
            m_anim.SetFloat(Move, Mathf.Abs(m_rb.linearVelocity.x));

            // Jump animation (if not grounded)
            m_anim.SetBool(IsJumping, !m_controller.isGrounded);
        }
    }
}
