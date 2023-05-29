using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class PlayerAnimator : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            UpdateFlip(collision.gameObject);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            UpdateFlip(collision.gameObject);
        }

        private void UpdateFlip(GameObject other)
        {
            if (!IsShadowObject(other)) return;

            if (other.transform.position.x < transform.position.x)
                Flip(true);
            else
                Flip(false);
        }

        private bool IsShadowObject(GameObject other)
        {
            return other.CompareTag("Shadow");
        }

        public void Flip(bool flag)
        {
            spriteRenderer.flipX = flag;
        }

        public void OnHit()
        {
            animator.Play("Hit");
        }

        public void OnLanding()
        {
            animator.Play("Landing");
        }

        public void PlayFallAnim()
        {
            animator.Play("Fall");
        }
    }
}
