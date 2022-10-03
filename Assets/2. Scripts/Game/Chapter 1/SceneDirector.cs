using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BeyondTheTale.Chapter1
{
    public class SceneDirector : MonoBehaviour
    {
        [SerializeField]
        private Transform player;
        [SerializeField]
        private Transform rabbit;
        [SerializeField]
        private Transform bed;
        [SerializeField]
        private Vector3 playerEndPos;
        [SerializeField]
        private Vector3 rabbitEndPos;
        [SerializeField]
        private ObstacleScroller obstacleScroller;
        [SerializeField]
        private float time;

        [SerializeField]
        private UnityEvent onStart = new UnityEvent();
        [SerializeField]
        private UnityEvent onMoveEnd = new UnityEvent();
        [SerializeField]
        private UnityEvent onHit = new UnityEvent();
        [SerializeField]
        private UnityEvent onLanding = new UnityEvent();
        [SerializeField]
        private UnityEvent onGameOver = new UnityEvent();
        [SerializeField]
        private UnityEvent onGameClear = new UnityEvent();

        private Vector3 playerPos;
        private Vector3 obstaclePos;
        private PlayerAnimator playerAnimator;
        private bool playerMove = false;

        private void Awake()
        {
            playerAnimator = player.GetComponent<PlayerAnimator>();
        }

        private IEnumerator Start()
        {
            onStart.Invoke();
            yield return StartCoroutine(MoveRoutine(rabbit, rabbit.position, rabbitEndPos, time));
            yield return StartCoroutine(MoveRoutine(player, player.position, playerEndPos, time));
            onMoveEnd.Invoke();
        }

        private IEnumerator MoveRoutine(Transform target, Vector3 start, Vector3 end, float t)
        {
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime / t;
                target.position = Vector3.Lerp(start, end, percent);

                yield return null;
            }
        }

        private void Restart()
        {
            FindObjectOfType<CameraBasedShadowDetector>()?.DestroyWebcamTexture();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnHit()
        {
            onHit.Invoke();
            playerAnimator.OnHit();
            obstacleScroller.Stop(true);
            Invoke(nameof(OnGameOver), 0.8f);
        }

        public void OnLanding()
        {
            onLanding.Invoke();
            MovePlayer(false);
            playerAnimator.OnLanding();
            obstacleScroller.Stop(true);
            onGameClear.Invoke();
        }

        public void OnGameOver()
        {
            onGameOver.Invoke();
            Invoke(nameof(Restart), 3f);
        }

        public void MovePlayer(bool value)
        {
            playerMove = value;
        }

        private void Update()
        {
            if (playerMove)
                player.Translate(Vector3.down * obstacleScroller.GetSpeed() * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(playerEndPos, 0.2f);
            Gizmos.DrawSphere(rabbitEndPos, 0.2f);
        }
    }
}