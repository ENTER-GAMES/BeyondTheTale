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
    private bool isRotating = false;

    private bool isLocked = false;

    public void Lock(bool value)
    {
        if (isLocked == value) return;

        isLocked = value;
    }

    public void StartRotate()
    {
        if (isRotating) return;

        StartCoroutine(nameof(RotateRoutine));
        isRotating = true;
    }

    public void StopRotate()
    {
        if (!isRotating) return;

        StopAllCoroutines();
        isRotating = false;
    }

    private IEnumerator RotateRoutine()
    {
        while (true)
        {
            if (!isLocked)
                transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

            yield return null;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Shadow"))
            onStayShadow.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Shadow"))
            onExitShadow.Invoke();
    }
}
