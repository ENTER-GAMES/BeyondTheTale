using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Larva : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField]
    private Sprite spriteIdle;              // Idle 스프라이트
    [SerializeField]
    private Sprite spriteHit;               // Hit 스프라이트

    [SerializeField]
    private float hitTime;                  // 맞는 동작 취하는 시간

    private bool isGameOver = false;        // 게임 오버 여부

    private SpriteRenderer spriteRenderer;
    private new ParticleSystem particleSystem;


    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void GameClear()
    {
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = 20;
    }

    public void Hit()
    {
        if (isGameOver) return;
        StopAllCoroutines();
        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        spriteRenderer.sprite = spriteHit;
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = 0;

        yield return new WaitForSeconds(hitTime);

        // 게임 오버 되었으면 코루틴 종료
        if (isGameOver) yield break;

        spriteRenderer.sprite = spriteIdle;
        emission.rateOverTime = 5.5f;
    }

    public void GameOver()
    {
        isGameOver = true;
        spriteRenderer.sprite = spriteHit;
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = 0;
    }
}
