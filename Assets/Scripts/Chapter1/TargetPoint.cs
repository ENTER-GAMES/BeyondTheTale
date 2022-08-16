using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class TargetPoint : MonoBehaviour
    {
        private SceneDirector sceneDirector;

        private void Awake()
        {
            sceneDirector = FindObjectOfType<SceneDirector>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                sceneDirector.OnLanding();
            }
        }
    }
}
