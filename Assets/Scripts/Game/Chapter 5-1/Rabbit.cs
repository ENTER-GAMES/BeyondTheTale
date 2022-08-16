using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chapter_5_1;

namespace Chapter_5_1
{
    public class Rabbit : Character
    {
        protected override void Awake()
        {
            base.Awake();
            moveDirection = Vector3.zero;
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);
            Turn(Vector3.right);
        }

        public override void Complete()
        {
            // 왼쪽 보기
            Turn(Vector3.left);
            Move();

            // 멈춤
            Turn(Vector3.zero);
        }
    }
}