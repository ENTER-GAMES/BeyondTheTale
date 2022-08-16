using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Book : MonoBehaviour
{
    [SerializeField]
    private Vector3 zoomInScale;
    [SerializeField]
    private float zoomInTime;
    [SerializeField]
    private AnimationCurve zoomInCurve;
    [SerializeField]
    private Vector3 zoomOutScale;
    [SerializeField]
    private float zoomOutTime;
    [SerializeField]
    private AnimationCurve zoomOutCurve;

    public void ZoomIn()
    {
        ZoomIn("");
    }

    public void ZoomIn(string targetSceneName)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomInRoutine(targetSceneName));
    }

    private IEnumerator ZoomInRoutine(string targetSceneName)
    {
        float timer = 0;
        float percent = 0;
        Vector3 startScale = transform.localScale;
        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / zoomInTime;

            transform.localScale = Vector3.Lerp(startScale, zoomInScale, zoomInCurve.Evaluate(percent));

            yield return null;
        }

        if (targetSceneName.Length > 0)
            SceneManager.LoadScene(targetSceneName);
    }

    public void ZoomOut()
    {
        StopAllCoroutines();
        StartCoroutine(nameof(ZoomOutRoutine));
    }

    private IEnumerator ZoomOutRoutine()
    {
        float timer = 0;
        float percent = 0;
        Vector3 startScale = transform.localScale;
        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / zoomOutTime;

            transform.localScale = Vector3.Lerp(startScale, zoomOutScale, zoomOutCurve.Evaluate(percent));

            yield return null;
        }
    }
}
