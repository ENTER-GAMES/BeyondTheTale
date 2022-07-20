using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class Obstacle : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                transform.parent.position = Vector3.zero;
                collision.transform.position = Vector3.zero;
            }
        }
    }
}
