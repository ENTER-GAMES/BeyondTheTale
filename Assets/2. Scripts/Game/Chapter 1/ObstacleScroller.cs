using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class ObstacleScroller : MonoBehaviour
    {
        [SerializeField]
        private float speed;
        private bool isStop = false;

        private void Update()
        {
            if (isStop) return;

            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        public void Stop(bool value)
        {
            isStop = value;
        }

        public float GetSpeed()
        {
            return speed;
        }
    }
}
