using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] private float innerTime;
    [SerializeField] private float outerTime;
    [SerializeField] private float innerDelay;
    [SerializeField] private float outerDelay;

    [SerializeField] private Vector3 innerStartScale;
    [SerializeField] private Vector3 innerEndScale;
    [SerializeField] private Vector3 outerStartScale;
    [SerializeField] private Vector3 outerEndScale;

    [SerializeField] private Transform innerCircle;
    [SerializeField] private Transform outerCircle;

    [SerializeField] private float colliderRadius;

    [SerializeField] private CircleTimeViewer circleTimeViewer;

    private CircleGenerator circleGenerator;
    private bool isEnd = false;
    private float innerLeftTime = 0;

    public void Init(CircleGenerator circleGenerator)
    {
        this.circleGenerator = circleGenerator;
    }

    public void Play()
    {
        isEnd = false;
        StartCoroutine(CircleRoutine());
        StartCoroutine(TimeRoutine());
    }

    private void Update()
    {
        if (!isEnd)
        {
            CheckShadow();
            circleTimeViewer.SetTime(innerLeftTime);
        }
    }

    private IEnumerator CircleRoutine()
    {
        yield return StartCoroutine(ScaleRoutine(innerCircle, innerStartScale, innerEndScale, innerTime));
        yield return StartCoroutine(EndRoutine());
    }

    private IEnumerator EndRoutine()
    {
        yield return StartCoroutine(ScaleRoutine(transform, outerStartScale, outerEndScale, outerTime));
        yield return new WaitForSeconds(outerDelay);
        yield return StartCoroutine(ScaleRoutine(transform, outerEndScale, outerStartScale, outerTime));
        yield return new WaitForSeconds(innerDelay);
        circleGenerator.onCircleFilled.Invoke(this);
    }

    private IEnumerator ScaleRoutine(Transform target, Vector3 start, Vector3 end, float time)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime / time;
            target.localScale = Vector3.Lerp(start, end, percent);
            yield return null;
        }
    }

    private IEnumerator TimeRoutine()
    {
        float current = 0;
        while (!isEnd)
        {
            current += Time.deltaTime;
            innerLeftTime = innerTime - current;
            yield return null;
        }
    }

    private void CheckShadow()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, colliderRadius, LayerMask.GetMask("Shadow Object"));

        if (collider != null)
        {
            isEnd = true;
            StopAllCoroutines();
            StartCoroutine(EndRoutine());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }
}
