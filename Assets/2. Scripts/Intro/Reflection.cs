using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Reflection : MonoBehaviour
{
    [SerializeField]
    private float maxReflectionSpeed = 0.2f;
    [SerializeField]
    private float reflectionTime = 2f;
    [SerializeField]
    private GameObject targetCutscene;
    private Vector3 targetCutsceneOriginPosition;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetCutsceneOriginPosition = targetCutscene.transform.position;
    }

    public void StartReflection()
    {
        Vector3 newPos = targetCutscene.transform.position;
        newPos.z = 30;
        targetCutscene.transform.position = newPos;

        spriteRenderer.enabled = true;
    }

    private IEnumerator ReflectionRoutine()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        float timer = 0;
        float percent = 0;
        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / reflectionTime;

            float speed = Mathf.Lerp(0, maxReflectionSpeed, percent);

            // spriteRenderer.material.SetFloat("_Reflection_Speed", speed);
            spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Reflection_Speed", speed);
            spriteRenderer.SetPropertyBlock(mpb);

            yield return null;
        }
    }

    public void StopReflection()
    {
        targetCutscene.transform.position = targetCutsceneOriginPosition;

        spriteRenderer.enabled = false;

        StopAllCoroutines();
    }
}
