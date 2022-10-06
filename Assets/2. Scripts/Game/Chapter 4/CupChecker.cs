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
    private float audioPlayTime = 0;    // audio 시작되고 경과된 시간

    [Header("Components")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private AudioSource audioSource;

    public bool IsSuccess() => currentCount >= successCount;

    private void Update()
    {
        audioPlayTime += Time.deltaTime;

        if (audioPlayTime > 0.5f && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            currentCount++;
            spriteRenderer.sprite = teaSprites[(int)Mathf.Lerp(0, teaSprites.Length - 1, (float)currentCount / successCount)];
            other.gameObject.SetActive(false);

            audioPlayTime = 0;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }
}
