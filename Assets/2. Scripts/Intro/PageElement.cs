using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PageElement : MonoBehaviour
{
    [Space]
    [SerializeField]
    protected UnityEvent onDisplay = new UnityEvent();
    [SerializeField]
    protected UnityEvent onHide = new UnityEvent();

    [SerializeField]
    private float displayDelayTime;     // 디스플레이 되기까지 기다리는 시간

    private void OnEnable()
    {
        StartCoroutine(nameof(DisplayRoutine));
    }

    public IEnumerator DisplayRoutine()
    {
        yield return new WaitForSeconds(displayDelayTime);
        onDisplay.Invoke();
    }

    private void OnDisable()
    {
        Hide();
    }

    private void Hide()
    {
        onHide.Invoke();
        StopAllCoroutines();
    }
}