using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class TargetPoint : MonoBehaviour
    {
        [SerializeField]
        private GameObject text;

        private void Start()
        {
            text.SetActive(false);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Time.timeScale = 0;
                text.SetActive(true);
            }
        }
    }
}
