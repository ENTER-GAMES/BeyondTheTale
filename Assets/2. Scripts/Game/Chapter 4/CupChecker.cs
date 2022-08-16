using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupChecker : MonoBehaviour
{
    [SerializeField]
    private string targetTag;
    [SerializeField]
    private int successCount;
    [SerializeField]
    private int currentCount = 0;
    [SerializeField]
    private bool isSuccess = false;

    public bool IsSuccess() => isSuccess;

    private void FixedUpdate()
    {
        isSuccess = currentCount >= successCount;
        currentCount = 0;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
            currentCount++;
    }

    // private void Update()
    // {

    // }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    // }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag(targetTag))
    //         currentCount--;
    // }
}
