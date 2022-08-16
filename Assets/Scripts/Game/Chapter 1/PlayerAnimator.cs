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

            Flip(other.transform);
        }

        private bool IsShadowObject(GameObject other)
        {
            return other.CompareTag("Shadow");
        }

        private void Flip(Transform other)
        {
            if (other.position.x < transform.position.x)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
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
