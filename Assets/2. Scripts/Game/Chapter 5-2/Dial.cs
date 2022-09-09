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
    private float rotateDelay;
    private bool isRotating = false;

    private bool isLocked = false;
    private bool flip = false;

    [SerializeField]
    private float radius;
    [SerializeField]
    private new Collider2D collider;

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

        StartCoroutine(nameof(RotateRoutine));
        isRotating = true;
    }

    public void StopRotate()
    {
        if (!isRotating) return;

        StopAllCoroutines();
        isRotating = false;
        flip = !flip;
    }

    private IEnumerator RotateRoutine()
    {
        float flipX = flip ? -1 : 1;
        while (true)
        {
            if (!isLocked)
                transform.Rotate(0, 0, rotateSpeed * flipX);
                //transform.Rotate(0, 0, rotateSpeed * flipX * Time.deltaTime);

            yield return new WaitForSeconds(rotateDelay);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //if (other.CompareTag("Shadow"))
        //    onStayShadow.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if (other.CompareTag("Shadow"))
        //    onExitShadow.Invoke();
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
