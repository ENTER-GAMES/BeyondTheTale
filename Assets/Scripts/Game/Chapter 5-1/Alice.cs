using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chapter_5_1;

namespace Chapter_5_1
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Alice : Character, ISpotLight2DTarget
    {
        public void OnHitBySpotLight2D()
        {
            Restart();
        }

        private void Restart()
        {
            rigidbody2D.velocity = Vector2.zero;
            transform.position = spawnPosition;
            moveDirection = Vector3.right;
        }
    }
}