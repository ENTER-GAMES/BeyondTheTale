using System.Collections;
using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class SceneDirector : MonoBehaviour
    {
        [SerializeField]
        private Transform player;
        [SerializeField]
        private new Transform camera;
        [SerializeField]
        private Transform obstacle;
        [SerializeField]
        private Vector3 endPos;
        [SerializeField]
        private float time;
        [SerializeField]
        private GameObject text;

        private Vector3 playerPos;
        private Vector3 cameraPos;
        private Vector3 obstaclePos;
        private PlayerAnimator playerAnimator;
        private ObstacleScroller obstacleScroller; 

        private void Start()
        {
            playerPos = player.position;
            cameraPos = camera.position;
            obstaclePos = obstacle.position;
            playerAnimator = player.GetComponent<PlayerAnimator>();
            obstacleScroller = obstacle.GetComponent<ObstacleScroller>();

            StartCoroutine(MoveRoutine());
        }

        private IEnumerator MoveRoutine()
        {
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime / time;
                player.position = Vector3.Lerp(playerPos, endPos, percent);

                yield return null;
            }
        }

        private void Restart()
        {
            player.position = playerPos;
            camera.position = cameraPos;
            obstacle.position = obstaclePos;
            obstacleScroller.IsStop(false);
            playerAnimator.PlayFallAnim();

            StartCoroutine(MoveRoutine());
        }

        public void OnHit()
        {
            playerAnimator.OnHit();
            obstacleScroller.IsStop(true);
            Invoke(nameof(Restart), 0.8f);
        }

        public void OnLanding()
        {
            playerAnimator.OnLanding();
            obstacleScroller.IsStop(true);
            text.SetActive(true);
        }
    }
}