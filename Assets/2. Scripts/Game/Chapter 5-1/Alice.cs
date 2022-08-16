using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chapter_5_1;

namespace Chapter_5_1
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Alice : Character, ISpotLight2DTarget
    {
        private bool isHit = false;             // 피격 상태 중 여부

        [Header("Audio")]
        [SerializeField]
        private AudioClip hitAudioClip;

        protected override void Awake()
        {
            base.Awake();

            // 처음에는 멈춰있도록 설정
            Turn(Vector3.zero);
        }

        public void StartMove()
        {
            Turn(Vector3.right);
        }

        public void OnHitBySpotLight2D()
        {
            Hit();
        }

        private void Hit()
        {
            if (isHit) return;
            isHit = true;

            // Hit 애니메이션 출력
            characterAnimator.Hit();

            // 멈추기
            Turn(Vector3.zero);

            // 2초 뒤에 리스타트
            Invoke(nameof(Restart), 2);

            // 오디오 재생
            sfxAudioSource?.PlayOneShot(hitAudioClip);
        }

        private void Restart()
        {
            isHit = false;

            // 애니메이션 초기화
            characterAnimator.Restart();

            // rigidbody 초기화
            rigidbody2D.velocity = Vector2.zero;

            // 초기 위치로 이동
            transform.position = spawnPosition;

            // 오른쪽으로 이동 시작
            Invoke(nameof(StartMove), 1);
        }

        public override void Complete()
        {
            // 멈춤
            Turn(Vector3.zero);
        }
    }
}