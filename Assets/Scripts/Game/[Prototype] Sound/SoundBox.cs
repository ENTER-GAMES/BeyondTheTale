using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundBox : MonoBehaviour
{
    [Serializable]
    private class Pitch
    {
        public AudioClip clip;
        public Color color;
    }

    [Header("Size")]
    [SerializeField]
    private Vector3 minSize;                // 최저 크기
    [SerializeField]
    private Vector3 maxSize;                // 최고 크기
    private Vector3 sizeGap;                // 단계별 간격

    [Header("Pitch & Volume")]
    [SerializeField]
    private Pitch[] pitches;                // 피치 정보 배열
    [SerializeField]
    private Pitch currentPitch;             // 현재 피치
    [SerializeField]
    private int volumeLevelCount;           // 볼륨 단계 수
    [SerializeField]
    private int currentVolumeLevel;         // 현재 볼륨 단계

    private AudioSource audioSource;
    [Header("Components")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Header("@Debug")]
    [SerializeField]
    private ShadowObject targetShadowObject;    // 사이즈를 기반으로 음을 생성할 타겟 그림자 오브젝트
    [SerializeField]
    private Vector2 targetShadowObjectSize;     // 타겟 그림자 오브젝트의 현재 크기


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        Init();
    }

    private void Init()
    {
        float volumeLevelSizeGap = (maxSize.x - minSize.x) / volumeLevelCount;
        float pitchSizeGap = (maxSize.y - minSize.y) / pitches.Length;

        sizeGap = new Vector3(volumeLevelSizeGap, pitchSizeGap);
    }

    private void Update()
    {
        UpdateBoxSize();
        UpdatePitchAndVolume();
        updateComponents();
    }

    private void UpdateBoxSize()
    {
        if (targetShadowObject == null)
        {
            transform.localScale = Vector3.zero;
            return;
        }

        Bounds bounds = targetShadowObject.bounds;
        transform.position = bounds.min;
        transform.localScale = bounds.size;
        targetShadowObjectSize = bounds.size;
    }

    private void UpdatePitchAndVolume()
    {
        if (targetShadowObject == null)
        {
            currentPitch = null;
            currentVolumeLevel = 0;
            return;
        }

        Bounds bounds = targetShadowObject.bounds;
        Vector2 size = bounds.size;

        int i = 0;
        for (; i < pitches.Length - 1; i++)
            if (size.y < minSize.y + (sizeGap.y * (i + 1))) break;

        currentPitch = pitches[i];

        i = 0;
        for (; i < volumeLevelCount; i++)
            if (size.x < minSize.x + (sizeGap.x * (i + 1))) break;

        currentVolumeLevel = i + 1;
    }

    private void updateComponents()
    {
        if (targetShadowObject == null)
        {
            audioSource.Stop();
            return;
        }

        audioSource.clip = currentPitch.clip;
        audioSource.volume = (1f / volumeLevelCount) * currentVolumeLevel;
        Color color = currentPitch.color;
        color.a = (180f + ((255f - 180f) / volumeLevelCount) * currentVolumeLevel) / 255f;
        spriteRenderer.color = color;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void SetTargetShadowObject(ShadowObject targetShadowObject)
    {
        this.targetShadowObject = targetShadowObject;
    }
}
