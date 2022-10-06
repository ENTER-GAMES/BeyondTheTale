using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dial : MonoBehaviour
{
    [SerializeField]
    private Dial[] childrenDials;

    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float rotateTime;
    [SerializeField]
    private float rotateDelay;
    private bool isRotating = false;

    private bool isLocked = false;
    private bool isClear = false;
    private bool flip = false;

    [SerializeField]
    private float radius;
    [SerializeField]
    private new Collider2D collider;
    private bool isStayShadow = false;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip audioClipRotateDial;

    private bool hasInitDone = false;

    public void Init()
    {
        hasInitDone = true;

        StartCoroutine(RotateRoutine());
    }

    public void Clear()
    {
        isClear = true;
    }

    public void Lock(bool value)
    {
        if (isLocked == value) return;

        isLocked = value;
    }

    private IEnumerator RotateRoutine()
    {
        float flipX = flip ? -1 : 1;
        while (true)
        {
            if (isClear)
                yield break;

            // 잠겨있지 않고, 그림자가 들어와 있으면
            if (!isLocked && isStayShadow)
            {
                // 돌고 있지 않았었으면 (새롭게 돌기 시작하면)
                // 회전 방향 변경
                if (!isRotating)
                    flipX *= -1;

                // 변수 초기화
                float timer = 0;
                float percent = 0;

                // 초기 각도
                Quaternion originRotation = transform.rotation;

                // 사운드 재생
                audioSource.PlayOneShot(audioClipRotateDial);

                // 회전 시작
                while (percent < 1)
                {
                    // 회전 중에 회전 루틴이 종료되어야 하고, 50%이상 넘어가지 않았으면
                    // 다시 뒤로(원상태로) 이동 후
                    // 회전 종료(break)
                    if ((isLocked || !isStayShadow) && percent < 0.5f)
                    {
                        Quaternion curRotation = transform.rotation;
                        float time = 0.3f;  // 0.3초간 뒤로감
                        timer = 0;
                        percent = 0;

                        while (percent < 1)
                        {
                            timer += Time.deltaTime;
                            percent = timer / time;
                            transform.rotation = Quaternion.Lerp(curRotation, originRotation, percent);
                            yield return null;
                        }

                        transform.rotation = originRotation;
                        isRotating = false;

                        break;
                    }

                    else
                    {
                        float timeAmount = (timer + Time.deltaTime >= rotateTime) ? rotateTime - timer : Time.deltaTime;
                        timer += timeAmount;
                        percent = timer / rotateTime;

                        transform.Rotate(0, 0, Mathf.Lerp(0, rotateSpeed * flipX, timeAmount / rotateTime));
                        isRotating = true;

                        yield return null;
                    }
                }
            }
            // 돌면 안될 때 (잠겨있거나 그림자가 없을 때)
            else
            {
                yield return null;
                isRotating = false;
            }

            if (isRotating)
                yield return new WaitForSeconds(rotateDelay);
        }
    }

    private void Update()
    {
        if (!hasInitDone) return;

        collider = Physics2D.OverlapCircle(transform.position, radius, LayerMask.GetMask("Shadow Object"));
        if (collider != null)
        {
            foreach (Dial dial in childrenDials)
                dial.Lock(true);

            isStayShadow = true;
        }
        else
        {
            foreach (Dial dial in childrenDials)
                dial.Lock(false);

            isStayShadow = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
