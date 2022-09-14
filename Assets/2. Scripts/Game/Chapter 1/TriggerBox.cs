using UnityEngine;
using UnityEngine.Events;

namespace BeyondTheTale.Chapter1
{
    public class TriggerBox : MonoBehaviour
    {
        [SerializeField]
        private string[] filterTags = new string[0];

        [SerializeField]
        private UnityEvent onTriggerEnter = new UnityEvent();
        [SerializeField]
        private UnityEvent onTriggerExit = new UnityEvent();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            foreach (string tag in filterTags)
            {
                if (collision.CompareTag(tag))
                    onTriggerEnter.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            foreach (string tag in filterTags)
            {
                if (collision.CompareTag(tag))
                    onTriggerExit.Invoke();
            }
        }
    }
}
