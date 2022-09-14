using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(BoxCollider2D))]
public class PageTrigger : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField]
    private Page[] pages;
    private int pageCount;
    [SerializeField]
    private int rayCountPerPage;
    [SerializeField]
    private float rayHeight;
    [SerializeField]
    private float pagingDelayTime;

    [Header("Feedback")]
    [SerializeField]
    private Light2D[] feedbackLights;
    [SerializeField]
    private float defaultIntensity = 10;
    [SerializeField]
    private float activeIntensity = 30;

    [Header("Target")]
    [SerializeField]
    private LayerMask targetLayerMask;

    [Header("Components")]
    private BoxCollider2D boxCollider2D;

    [Header("@Debug")]
    [SerializeField]
    private int targetPageIndex = -1;
    [SerializeField]
    private int currentPageIndex = -1;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        pageCount = pages.Length;
    }

    private void Start()
    {
        transform.localScale = Vector3.one;
        boxCollider2D.offset = Vector2.zero;

        StartCoroutine(TriggerRoutine());
        StartCoroutine(TurnRoutine());
    }

    private IEnumerator TriggerRoutine()
    {
        // * 계속 돌고있는 코루틴
        // 그림자 들어오면, 목표 페이지 인덱스를 업데이트 해줌

        WaitForSeconds wait = new WaitForSeconds(1f / 60f);

        while (true)
        {
            int target = FindTargetPage();
            if (target >= 0)
            {
                targetPageIndex = target;

                // 피드백 불빛 활성화
                foreach (Light2D light in feedbackLights)
                    light.intensity = defaultIntensity;

                feedbackLights[target].intensity = activeIntensity;
            }

            yield return wait;
        }
    }

    private IEnumerator TurnRoutine()
    {
        // * 계속 돌고있는 코루틴
        // 현재 페이지 인덱스랑 목표 페이지 인덱스가 다르면, 그 쪽 방향으로 책을 넘겨줌

        WaitForSeconds wait = new WaitForSeconds(1f / 60f);

        while (true)
        {
            if (currentPageIndex != targetPageIndex)
            {
                int dir = (int)Mathf.Sign(targetPageIndex - currentPageIndex);
                yield return StartCoroutine(TurnByDirectionRoutine(dir));
            }

            yield return wait;
        }
    }

    private IEnumerator TurnByDirectionRoutine(int direction)
    {
        // 한 방향으로 페이지가 넘어가는 것만 체크하는 코루틴입니다.
        // 매개변수 direction 방향으로 targetPageIndex가 변경될 경우 적용되지만
        // 반대 방향으로 targetPageIndex가 변경될 경우, 본 코루틴이 멈추게 됩니다.
        // 단, 페이지 넘기기가 모두 끝나고 멈춥니다.

        // 넘기기 시작하면 모든 페이지를 숨긴다.
        HideAllPages();

        while (true)
        {
            // 다음 페이지
            int nextPageIndex = currentPageIndex + direction;
            Page nextPage = pages[direction >= 0 ? nextPageIndex : nextPageIndex + 1];

            // 페이지 넘기기 코루틴 실행
            // 페이지마다 설정된 딜레이 후, 코루틴 종료됨
            yield return StartCoroutine(nextPage.TurnRoutine(direction));

            // 현재 페이지 인덱스 갱신
            currentPageIndex = nextPageIndex;

            // 만약 목표 인덱스에 도달했거나, targetPageIndex가 direction과 반대로 변경되었을 경우
            // 페이지 넘기기가 끝날 때까지 기다려주고 코루틴 종료
            if (currentPageIndex == targetPageIndex || Mathf.Sign(targetPageIndex - currentPageIndex) != direction)
            {
                // continue해야 하는지 여부
                bool continueFlag = false;

                // 마지막으로 넘긴 페이지가 다 넘겨질 때까지 기다림
                while (!nextPage.isEnd)
                {
                    // 기다리는 도중 direction 방향으로 targetPageIndex가 업데이트되면 기다림을 중지하고
                    // 다시 페이지 넘길 수 있도록 continue한다.
                    if (currentPageIndex != targetPageIndex && Mathf.Sign(targetPageIndex - currentPageIndex) == direction)
                    {
                        continueFlag = true;
                        break;
                    }

                    yield return null;
                }

                if (continueFlag)
                    continue;
                else
                {
                    // 만약 현재 서있는 목표 페이지에 도달해서 끝나는 것이라면
                    // 해당 페이지 출력
                    if (currentPageIndex == targetPageIndex)
                        DisplayPage(currentPageIndex);

                    yield break;
                }
            }

            yield return null;
        }
    }

    private int FindTargetPage()
    {
        Vector3 startPoint = GetStartRayPoint();
        float gap = GetRayGap();
        int rayCount = pageCount * rayCountPerPage;

        for (int i = 0; i < rayCount; i++)
        {
            Vector3 point = startPoint + (transform.right * gap * i);
            RaycastHit2D hit = Physics2D.Raycast(point, transform.up, rayHeight, targetLayerMask);

            if (hit.collider == null) continue;

            return i / rayCountPerPage;
        }

        return -1;
    }

    private Vector3 GetStartRayPoint()
    {
        Vector3 size = boxCollider2D.size;
        Vector3 center = transform.position;

        Vector3 startPoint = center + (-transform.right * (size.x / 2)) + (transform.up * (size.y / 2));

        return startPoint;
    }

    private float GetRayGap()
    {
        return boxCollider2D.size.x / (pageCount * rayCountPerPage - 1);
    }

    private void DisplayPage(int targetPageIndex)
    {
        // 모든 페이지 숨기고
        HideAllPages();

        // 목표 페이지만 디스플레이
        pages[targetPageIndex]?.DisplayPage();
    }

    private void HideAllPages()
    {
        // 출력되고 있는 모든 페이지 숨김
        foreach (Page page in pages)
            page?.HidePage();

    }

    private void OnDrawGizmos()
    {
        if (boxCollider2D == null) return;

        Vector3 startPoint = GetStartRayPoint();
        float gap = GetRayGap();
        int rayCount = pageCount * rayCountPerPage;

        for (int i = 0; i < rayCount; i++)
        {
            Vector3 point = startPoint + (transform.right * gap * i);

            if (currentPageIndex == (int)(i / rayCountPerPage))
                Gizmos.color = Color.red;
            else if (targetPageIndex == (int)(i / rayCountPerPage))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.yellow;

            Gizmos.DrawLine(point, point + (transform.up * rayHeight));
        }
    }
}
