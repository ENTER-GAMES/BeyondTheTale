using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class TargetPoint : MonoBehaviour
    {
        private SceneDirector sceneDirector;

        private Animator animator;

        private void Awake()
        {
            sceneDirector = FindObjectOfType<SceneDirector>();

            animator = GetComponent<Animator>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                sceneDirector.OnLanding();

                PressedBed();
            }
        }

        public void PressedBed()
        {
            animator.Play("Pressed", -1);
        }
    }
}
