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

    [SerializeField]
    private bool isLanding = false;      // 착지 중인지 여부 (땅에 닿았을 때부터 착지 애니 끝날 때까지 true)
    public bool IsLanding => isLanding;

    private bool isHit = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (velocity.x != 0)
        {
            spriteRenderer.flipX = velocity.x > 0 || isHit;
        }

        animator.SetBool("isMoving", velocity.x != 0);
        animator.SetFloat("velocityY", velocity.y);
    }

    public void Jump()
    {
        animator.Play("Jump", -1);
    }

    public void Land()
    {
        isLanding = true;
        animator.Play("Landing", -1);
    }

    public void OnFinishedLoading()
    {
        isLanding = false;
    }

    public void Restart()
    {
        isHit = false;
        animator.SetBool("isHit", isHit);
    }

    public void Hit()
    {
        isHit = true;
        animator.SetBool("isHit", isHit);
        animator.Play("Hit", -1);
    }
}
