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

    [Header("Tea")]
    [SerializeField]
    private Sprite[] teaSprites;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public bool IsSuccess() => currentCount >= successCount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            currentCount++;
            spriteRenderer.sprite = teaSprites[(int)Mathf.Lerp(0, teaSprites.Length - 1, (float)currentCount / successCount)];
            other.gameObject.SetActive(false);
        }
    }
}
