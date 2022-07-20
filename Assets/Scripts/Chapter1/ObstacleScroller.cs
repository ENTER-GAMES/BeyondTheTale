using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class ObstacleScroller : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        private void Update()
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }
}
