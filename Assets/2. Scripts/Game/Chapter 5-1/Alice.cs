using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Chapter_5_1;

namespace Chapter_5_1
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Alice : Character, ISpotLight2DTarget
    {
        public UnityEvent onHit = new UnityEvent();

        private bool isHit = false;             // 피격 상태 중 여부

        [Header("Audio")]
        [SerializeField]
        private AudioClip hitAudioClip;

        private new Collider2D collider2D;

        protected override void Awake()
        {
            base.Awake();

            collider2D = GetComponent<Collider2D>();

            // 처음에는 멈춰있도록 설정
            Turn(Vector3.zero);
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.R))
                OnHitBySpotLight2D();
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

            // 부모 트랜스폼 초기값으로 변경
            transform.parent = originParentTransform;

            // Hit 애니메이션 출력
            characterAnimator.Hit();

            // 멈추기
            Turn(Vector3.zero);
            rigidbody2D.velocity = Vector2.zero;

            // 콜라이더 비활성화
            collider2D.enabled = false;

            // 2초 뒤에 리스타트
            Invoke(nameof(Restart), 2);

            // 오디오 재생
            sfxAudioSource?.PlayOneShot(hitAudioClip);

            // 이벤트 호출
            onHit.Invoke();
        }

        private void Restart()
        {
            isHit = false;

            // 애니메이션 초기화
            characterAnimator.Restart();

            // rigidbody 초기화
            rigidbody2D.velocity = Vector2.zero;

            // 콜라이더 활성화
            collider2D.enabled = true;

            // 초기 위치로 이동
            transform.position = spawnPosition;

            // 오른쪽으로 이동 시작
            Invoke(nameof(StartMove), 1);
        }

        public override void Complete()
        {
            // 멈춤
            // Turn(Vector3.zero);
        }
    }
}