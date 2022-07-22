using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chapter_5_1;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (velocity.x != 0)
        {
            spriteRenderer.flipX = velocity.x > 0 ? true : false;
        }

        animator.SetBool("isMoving", velocity.x != 0);
        animator.SetFloat("velocityY", velocity.y);
    }

    public void Jump()
    {
        animator.SetBool("isJumping", true);
        animator.Play("Jump", -1);
    }

    public void Land()
    {
        animator.SetBool("isJumping", false);
    }
}
