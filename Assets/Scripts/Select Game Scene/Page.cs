using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Page : MonoBehaviour
{
    [SerializeField]
    private int leftSideZIndex;
    [SerializeField]
    private int rightSideZIndex;
    [SerializeField]
    private float delayTime;

    public bool isEnd { private set; get; }

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        isEnd = true;
    }

    public void RightSide()
    {
        Vector3 newPos = transform.position;
        newPos.z = rightSideZIndex;
        transform.position = newPos;
    }

    public void LeftSide()
    {
        Vector3 newPos = transform.position;
        newPos.z = leftSideZIndex;
        transform.position = newPos;
    }

    private void StartTrunOver()
    {
        spriteRenderer.enabled = true;
        isEnd = false;
    }

    public void EndTrunOver()
    {
        spriteRenderer.enabled = false;
        isEnd = true;
    }

    public IEnumerator TurnRoutine(int direction)
    {
        string animationName = direction >= 0 ? "Next" : "Previous";
        animator.Play(animationName, -1);

        StartTrunOver();

        yield return new WaitForSeconds(delayTime);
    }
}
