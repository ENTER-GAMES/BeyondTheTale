using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeyondTheTale.Chapter1
{
    public class Obstacle : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
