using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PageElement : MonoBehaviour
{
    [SerializeField]
    protected UnityEvent onDisplay = new UnityEvent();
    [SerializeField]
    protected UnityEvent onHide = new UnityEvent();

    [SerializeField]
    private float displayDelayTime;

    private void OnEnable()
    {
        StartCoroutine(nameof(DisplayRoutine));
    }

    private IEnumerator DisplayRoutine()
    {
        yield return new WaitForSeconds(displayDelayTime);
        onDisplay.Invoke();
    }

    private void OnDisable()
    {
        onHide.Invoke();
    }
}
