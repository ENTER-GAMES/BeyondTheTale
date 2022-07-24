using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chapter_5_1;

namespace Chapter_5_1
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Alice : Character, ISpotLight2DTarget
    {
        protected override void Awake()
        {
            base.Awake();

            // 처음에는 멈춰있도록 설정
            moveDirection = Vector3.zero;
        }

        public void StartMove()
        {
            moveDirection = Vector3.right;
        }

        public void OnHitBySpotLight2D()
        {
            // 빛에 맞으면 재시작
            Restart();
        }

        private void Restart()
        {
            rigidbody2D.velocity = Vector2.zero;
            transform.position = spawnPosition;
            moveDirection = Vector3.right;
        }

        public override void Complete()
        {
            // 멈춤
            Turn(Vector3.zero);
        }
    }
}