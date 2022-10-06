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
    private int leftSideZIndex;     // 왼쪽에 갔을 때 z index
    [SerializeField]
    private int rightSideZIndex;    // 오른쪽에 갔을 때 z index
    [SerializeField]
    private float delayTime;        // 책을 넘길 수 있는 상태가 되는 시간

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
        // 책 넘기는 루틴
        // 애니메이션 재생 후
        // 다음 페이지가 넘어갈 수 있도록, delayTime이 지난 후에 코루틴 끝남

        string animationName = direction >= 0 ? "Next" : "Previous";
        animator.Play(animationName, -1);

        StartTrunOver();

        yield return new WaitForSeconds(delayTime);
    }

    public void DisplayPage()
    {
        isDisplay = true;
        SetActiveAllPageElements(true);
    }

    public void HidePage()
    {
        isDisplay = false;
        SetActiveAllPageElements(false);
    }

    private void SetActiveAllPageElements(bool value)
    {
        foreach (PageElement pageElement in pageElements)
            pageElement.gameObject.SetActive(value);
    }
}
