using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chapter_5_1
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Character : MonoBehaviour, IJumpable, ITurnable, ICompletable
    {
        protected Transform originParentTransform;          // 초기 부모 트랜스폼
        protected Vector3 spawnPosition;                    // 스폰 위치 (처음 위치)

        [Header("Move")]
        [SerializeField]
        protected float moveSpeed = 1f;                     // 이동 속도
        protected float moveSpeedDuringJump = 0;            // 점프하는 동안의 이동 속도
        protected Vector3 moveDirection = Vector3.right;    // 이동 방향

        [Header("Jump")]
        [SerializeField]
        protected float jumpForce = 5f;                     // 점프 힘
        protected bool isJumping = false;                   // 점프 중인지 여부
        [SerializeField]
        protected float jumpDelayTime = 0.25f;              // 점프 앞에 쉬는 시간

        [Header("Components")]
        [SerializeField]
        protected CharacterAnimator characterAnimator;
        protected new Rigidbody2D rigidbody2D;

        [Header("Audio")]
        [SerializeField]
        protected AudioClip jumpAudioClip;
        [SerializeField]
        protected AudioSource sfxAudioSource;
        [SerializeField]
        protected AudioSource moveAudioSource;

        [Header("@Debug")]
        [SerializeField]
        protected bool isGround = false;                    // 바닥에 닿았는지 여부


        protected virtual void Awake()
        {
            originParentTransform = transform.parent;

            rigidbody2D = GetComponent<Rigidbody2D>();
            SpawnPositionIsCurrentPosition();
        }

        #region Update
        protected virtual void Update()
        {
            Move();
        }

        public void SpawnPositionIsCurrentPosition()
        {
            spawnPosition = transform.position;
        }

        protected void Move()
        {
            float moveSpeed = isJumping ? moveSpeedDuringJump : this.moveSpeed;
            Vector2 moveAmount = moveDirection * moveSpeed;
            rigidbody2D.velocity = new Vector2(moveAmount.x, rigidbody2D.velocity.y);

            // 애니메이션 업데이트
            characterAnimator?.SetVelocity(rigidbody2D.velocity);

            // 뛰는(걷는) 오디오 재생
            UpdateMoveAudio();
        }

        protected void UpdateMoveAudio()
        {
            if (moveAudioSource == null) return;

            bool isMoving = rigidbody2D.velocity.x != 0;

            if ((isJumping || (!isJumping && !isMoving)) && moveAudioSource.isPlaying)
                moveAudioSource.Stop();
            else if (!isJumping && isMoving && !moveAudioSource.isPlaying)
                moveAudioSource.Play();
        }
        #endregion

        #region Jump
        public virtual void Jump() => Jump(jumpForce);

        public virtual void Jump(float jumpForce) => Jump(jumpForce, moveSpeed);

        public virtual void Jump(float jumpForce, float moveSpeedDuringJump)
        {
            // 땅에 닿지 않았거나, 점프 중이면 리턴
            if (!isGround || isJumping) return;

            StartCoroutine(JumpRoutine(jumpForce, moveSpeedDuringJump));
        }

        private IEnumerator JumpRoutine(float jumpForce, float moveSpeedDuringJump)
        {
            // 상태 업데이트
            isGround = false;
            isJumping = true;
            transform.parent = originParentTransform;

            // 점프 중 이동속도 설정
            this.moveSpeedDuringJump = moveSpeedDuringJump;

            // 애니메이션 재생
            characterAnimator?.Jump();

            // 점프 딜레이가 있으면 멈춰서 쉬고 다시 진행
            if (jumpDelayTime > 0)
            {
                Vector3 temp = moveDirection;

                Stop();
                yield return new WaitForSeconds(jumpDelayTime);
                Turn(temp);
            }

            // 점프
            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // 오디오 재생
            sfxAudioSource?.PlayOneShot(jumpAudioClip);
        }

        private IEnumerator LandingRoutine()
        {
            if (characterAnimator == null) yield break;
            if (!characterAnimator.IsLanding) yield break;

            Vector3 temp = moveDirection;

            Stop();

            while (true)
            {
                if (!characterAnimator.IsLanding) break;
                yield return null;
            }
            Turn(temp);
        }
        #endregion

        #region Turn
        public void Stop() => Turn(Vector3.zero);

        public void Turn() => Turn(moveDirection * -1);  // 반대 방향으로 설정

        public void Turn(Vector3 moveDirection)
        {
            this.moveDirection = moveDirection;
        }
        #endregion

        #region Complete
        public virtual void Complete() { }
        #endregion

        #region Collision
        protected void OnCollisionEnter2D(Collision2D other)
        {
            if (isGround) return;

            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Movable Ground"))
            {
                bool isLanding = isJumping;

                isGround = true;
                isJumping = false;

                if (other.gameObject.CompareTag("Movable Ground"))
                {
                    transform.parent = other.transform;
                }

                if (characterAnimator == null) return;
                if (!isLanding) return;

                characterAnimator.Land();
                StartCoroutine(nameof(LandingRoutine));    // 애니메이션 끝날 때까지 정지
            }
        }

        protected void OnCollisionExit2D(Collision2D other)
        {
            // if (other.gameObject.CompareTag("Ground"))
            // isGround = false;
        }
        #endregion
    }
}