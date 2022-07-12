using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        [SerializeField]
        private float smoothTime;
        [SerializeField]
        private Vector2 minCameraPos;
        [SerializeField]
        private Vector2 maxCameraPos;

        private float velocity;

        private void FixedUpdate()
        {
            float x = Mathf.Clamp(transform.position.x, minCameraPos.x, maxCameraPos.x);
            float y = Mathf.SmoothDamp(transform.position.y, target.position.y, ref velocity, smoothTime);
            y = Mathf.Clamp(y, minCameraPos.y, maxCameraPos.y);

            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}