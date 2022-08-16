using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Page : MonoBehaviour
{
    public UnityEvent onStartTurnOver = new UnityEvent();
    public UnityEvent onEndTurnOver = new UnityEvent();

    [SerializeField]
    private PageElement[] pageElements;
    [SerializeField]
    private int leftSideZIndex;
    [SerializeField]
    private int rightSideZIndex;
    [SerializeField]
    private float delayTime;

    public bool isEnd { get; private set; }
    public bool isDisplay { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        isEnd = true;
        isDisplay = false;
    }

    private void Start()
    {
        HidePage();
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
        onStartTurnOver.Invoke();
    }

    public void EndTrunOver()
    {
        spriteRenderer.enabled = false;
        isEnd = true;
        onEndTurnOver.Invoke();
    }

    public IEnumerator TurnRoutine(int direction)
    {
        string animationName = direction >= 0 ? "Next" : "Previous";
        animator.Play(animationName, -1);

        StartTrunOver();

        yield return new WaitForSeconds(delayTime);
    }

    public void DisplayPage()
    {
        foreach (PageElement pageElement in pageElements)
            pageElement.gameObject.SetActive(true);

        isDisplay = true;
    }

    public void HidePage()
    {
        foreach (PageElement pageElement in pageElements)
            pageElement.gameObject.SetActive(false);

        isDisplay = false;
    }
}
