using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chapter_5_1
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Character : MonoBehaviour, IJumpable, ITurnable, ICompletable
    {
        protected Vector3 spawnPosition;                    // 스폰 위치 (처음 위치)
        [SerializeField]
        protected float moveSpeed = 1f;                     // 이동 속도
        protected float moveSpeedDuringJump = 0;            // 점프하는 동안의 이동 속도
        protected Vector3 moveDirection = Vector3.right;    // 이동 방향
        [SerializeField]
        protected float jumpForce = 5f;                     // 점프 힘
        protected bool isJumping = false;                   // 점프 중인지 여부
        protected bool isGround = false;                    // 바닥에 닿았는지 여부

        protected new Rigidbody2D rigidbody2D;
        [SerializeField]
        protected CharacterAnimator characterAnimator;

        protected virtual void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            spawnPosition = transform.position;
        }

        #region Update
        protected void Update()
        {
            Move();
        }

        protected void Move()
        {
            float moveSpeed = isJumping ? moveSpeedDuringJump : this.moveSpeed;
            Vector2 moveAmount = moveDirection * moveSpeed;
            rigidbody2D.velocity = new Vector2(moveAmount.x, rigidbody2D.velocity.y);

            characterAnimator?.SetVelocity(rigidbody2D.velocity);
        }
        #endregion

        #region Jump
        public void Jump() => Jump(jumpForce);

        public void Jump(float jumpForce) => Jump(jumpForce, moveSpeed);

        public void Jump(float jumpForce, float moveSpeedDuringJump)
        {
            // 땅에 닿지 않았거나, 점프 중이면 리턴
            if (!isGround || isJumping) return;

            // 상태 업데이트
            isGround = false;
            isJumping = true;

            // 점프 중 이동속도 설정
            this.moveSpeedDuringJump = moveSpeedDuringJump;

            // 점프
            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            characterAnimator?.Jump();
        }
        #endregion

        #region Turn
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
            if (other.gameObject.CompareTag("Ground"))
            {
                isGround = true;
                isJumping = false;

                characterAnimator?.Land();
            }
        }

        protected void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground"))
                isGround = false;
        }
        #endregion
    }
}