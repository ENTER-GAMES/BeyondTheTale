using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CatManager : MonoBehaviour
{
    public UnityEvent onDoorOpen = new UnityEvent();
    public UnityEvent<int> onScream = new UnityEvent<int>();
    public UnityEvent onGameClear = new UnityEvent();
    public UnityEvent onGameOver = new UnityEvent();

    private int hitCount = 0;
    private int screamCount = 0;

    [SerializeField]
    private List<Cat> cats;
    private List<Cat> activeCats;
    private List<Cat> deactiveCats;

    [Header("Level")]
    [SerializeField]
    private int startActiveCount = 1;       // 소환할 고양이 시작 수
    [SerializeField]
    private int endActiveCount = 4;         // 소환할 고양이 끝 수
    [SerializeField]
    private float nextLevelTime = 8;        // 다음 단계로 가는 시간

    private int targetActiveCount = 1;      // 소환할 목표 고양이 수


    [SerializeField]
    private int gameClearCount = 30;        // 게임 클리어 기준이 되는 고양이 처치 수
    [SerializeField]
    private int gameOverCount = 3;          // 게임 오버 기준이 되는 고양이 고함 수

    private void Awake()
    {
        targetActiveCount = startActiveCount;
    }

    private void Start()
    {
        foreach (Cat cat in cats)
            cat.Init(this);

        activeCats = new List<Cat>();
        deactiveCats = new List<Cat>();

        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        for (int i = startActiveCount; i <= endActiveCount; i++)
        {
            targetActiveCount = i;
            yield return new WaitForSeconds(nextLevelTime);
        }
    }

    private void Activate()
    {
        if (activeCats.Count != 0)
            return;

        if (hitCount >= gameClearCount)
        {
            GameClear();
            return;
        }

        for (int i = activeCats.Count; i < targetActiveCount; i++)
        {
            Cat cat = deactiveCats[Random.Range(0, deactiveCats.Count)];
            deactiveCats.Remove(cat);
            activeCats.Add(cat);
            cat.Activate();
        }
    }

    public void OnDoorOpen()
    {
        onDoorOpen.Invoke();
    }

    public void OnHitCat()
    {
        hitCount++;
    }

    public void OnScream()
    {
        screamCount++;
        onScream.Invoke(screamCount);

        if (screamCount == gameOverCount)
            GameOver();
    }

    private void GameClear()
    {
        onGameClear.Invoke();
    }

    private void GameOver()
    {
        onGameOver.Invoke();

        Invoke(nameof(Restart), 3);     // 3초 뒤, 재시작
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClose(Cat cat)
    {
        activeCats.Remove(cat);
        deactiveCats.Add(cat);
        Activate();
    }
}
