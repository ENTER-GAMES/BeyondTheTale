using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chapter_5_1
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Character : MonoBehaviour, IJumpable, ITurnable
    {
        protected Vector3 spawnPosition;
        [SerializeField]
        protected float moveSpeed = 1f;
        protected float moveSpeedDuringJump = 0;
        protected Vector3 moveDirection = Vector3.right;
        [SerializeField]
        protected float jumpForce = 5f;
        protected bool isJumping = false;
        protected bool isGround = false;

        #region Component
        protected new Rigidbody2D rigidbody2D;
        #endregion

        protected void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            spawnPosition = transform.position;
        }

        protected void Update()
        {
            Move();
        }

        protected void Move()
        {
            float moveSpeed = isJumping ? moveSpeedDuringJump : this.moveSpeed;
            Vector2 moveAmount = moveDirection * moveSpeed;
            rigidbody2D.velocity = new Vector2(moveAmount.x, rigidbody2D.velocity.y);
        }

        public void Jump() => Jump(jumpForce);

        public void Jump(float jumpForce) => Jump(jumpForce, moveSpeed);

        public void Jump(float jumpForce, float moveSpeedDuringJump)  // moveSpeed: 점프할 동안의 moveSpeed
        {
            if (!isGround || isJumping) return;

            isGround = false;
            isJumping = true;

            this.moveSpeedDuringJump = moveSpeedDuringJump;

            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        public void Turn() => Turn(moveDirection * -1);

        public void Turn(Vector3 moveDirection)
        {
            this.moveDirection = moveDirection;
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                isGround = true;
                isJumping = false;
            }
        }

        protected void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground"))
                isGround = false;
        }
    }
}