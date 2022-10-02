using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HitEffect : MonoBehaviour
{
    [SerializeField]
    private float hitTime;
    [SerializeField]
    private AnimationCurve effectCurve;

    private Volume volume;

    private void Awake()
    {
        volume = GetComponent<Volume>();
    }

    public void Hit()
    {
        StopAllCoroutines();
        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        if (volume.profile.TryGet<ChromaticAberration>(out ChromaticAberration component))
        {
            float timer = 0;
            float percent = 0;

            while (percent < 1)
            {
                timer += Time.deltaTime;
                percent = timer / hitTime;

                component.intensity.value = effectCurve.Evaluate(percent);

                yield return null;
            }
        }
    }
}
