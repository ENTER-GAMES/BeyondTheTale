using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dial : MonoBehaviour
{
    public UnityEvent onStayShadow = new UnityEvent();
    public UnityEvent onExitShadow = new UnityEvent();

    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float rotateTime;
    [SerializeField]
    private float rotateDelay;
    private bool isRotating = false;

    private bool isLocked = false;
    private bool flip = false;
    private bool breakRotateRotine = false;

    [SerializeField]
    private float radius;
    [SerializeField]
    private new Collider2D collider;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip audioClipRotateDial;

    private bool hasInitDone = false;

    public void Init()
    {
        hasInitDone = true;
    }

    public void Lock(bool value)
    {
        if (isLocked == value) return;

        isLocked = value;
    }

    public void StartRotate()
    {
        if (isRotating) return;

        breakRotateRotine = false;
        StartCoroutine(nameof(RotateRoutine));
        isRotating = true;
    }

    public void StopRotate()
    {
        if (!isRotating) return;

        // StopAllCoroutines();     // 중간에 끊으면 각 안맞음
        // isRotating = false;
        breakRotateRotine = true;
        flip = !flip;
    }

    private IEnumerator RotateRoutine()
    {
        float flipX = flip ? -1 : 1;
        while (true)
        {
            if (!isLocked)
            {
                float timer = 0;
                float percent = 0;
                audioSource.PlayOneShot(audioClipRotateDial);

                while (percent < 1)
                {
                    float timeAmount = (timer + Time.deltaTime >= rotateTime) ? rotateTime - timer : Time.deltaTime;
                    timer += timeAmount;
                    percent = timer / rotateTime;

                    transform.Rotate(0, 0, Mathf.Lerp(0, rotateSpeed * flipX, timeAmount / rotateTime));
                    yield return null;
                }
            }

            if (breakRotateRotine)
            {
                isRotating = false;
                yield break;
            }
            else
                yield return new WaitForSeconds(rotateDelay);
        }
    }

    private void Update()
    {
        if (!hasInitDone) return;
        if (isLocked) return;

        collider = Physics2D.OverlapCircle(transform.position, radius, LayerMask.GetMask("Shadow Object"));
        if (collider != null)
            onStayShadow.Invoke();
        else
            onExitShadow.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
