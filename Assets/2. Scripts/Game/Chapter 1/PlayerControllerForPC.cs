using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeyondTheTale.Chapter1
{
    public class PlayerControllerForPC : MonoBehaviour
    {
        private new Rigidbody2D rigidbody2D;
        private PlayerAnimator playerAnimator;

        [SerializeField]
        private float speed = 5;

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            playerAnimator = GetComponent<PlayerAnimator>();
        }

        private void FixedUpdate()
        {
            Vector3 direction = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

            rigidbody2D.MovePosition(transform.position + direction * speed * Time.deltaTime);

            if (direction.x > 0)
                playerAnimator.Flip(true);
            else if (direction.x < 0)
                playerAnimator.Flip(false);
        }
    }
}
