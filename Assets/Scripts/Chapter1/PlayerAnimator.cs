using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

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
    }
}
