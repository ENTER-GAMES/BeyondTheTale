using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CatManager : MonoBehaviour
{
    public UnityEvent onDoorOpen = new UnityEvent();
    public UnityEvent onHitCat = new UnityEvent();
    public UnityEvent<int> onHitRabbit = new UnityEvent<int>();
    public UnityEvent<int> onScream = new UnityEvent<int>();
    public UnityEvent onArrivedHalfLine = new UnityEvent();
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
    [Range(0, 100)]
    [SerializeField]
    private int rabbitChance = 10;          // 토끼 등장 확률
    [SerializeField]
    private float nextLevelTime = 8;        // 다음 단계로 가는 시간

    private int targetActiveCount = 1;      // 소환할 목표 고양이 수


    [SerializeField]
    private int halfLineCount = 10;         // 중간 지점 기준 개수 (고양이 남은 수)
    [SerializeField]
    private int gameClearCount = 30;        // 게임 클리어 기준이 되는 고양이 처치 수
    [SerializeField]
    private int gameOverCount = 3;          // 게임 오버 기준이 되는 고양이 고함 수
    private int currentGameOverCount = 0;

    private bool hasArrivedHalfLine = false;    // 중간 지점 넘었는지 여부
    private bool isGameOver = false;            // 게임오버 여부

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

        if (gameClearCount - hitCount <= halfLineCount)
        {
            ArriveHalfLine();
        }

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
            if (rabbitChance > Random.Range(0, 100))
                cat.Activate(WindowState.Rabbit);
            else
                cat.Activate(WindowState.Cat);
        }
    }

    public void OnDoorOpen()
    {
        onDoorOpen.Invoke();
    }

    public void OnHitCat()
    {
        hitCount++;
        onHitCat.Invoke();
    }

    public void OnScream()
    {
        screamCount++;
        currentGameOverCount++;
        onScream.Invoke(currentGameOverCount);

        if (currentGameOverCount >= gameOverCount)
            GameOver();
    }

    public void OnHitRabbit()
    {
        currentGameOverCount++;
        onHitRabbit.Invoke(currentGameOverCount);

        if (currentGameOverCount >= gameOverCount)
            GameOver();
    }

    private void ArriveHalfLine()
    {
        if (hasArrivedHalfLine) return;
        hasArrivedHalfLine = true;

        onArrivedHalfLine.Invoke();
    }

    private void GameClear()
    {
        if (isGameOver) return;

        onGameClear.Invoke();
    }

    private void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        onGameOver.Invoke();
        Invoke(nameof(Restart), 3);     // 3초 뒤, 재시작
    }

    private void Restart()
    {
        FindObjectOfType<CameraBasedShadowDetector>()?.Dispose();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClose(Cat cat)
    {
        activeCats.Remove(cat);
        deactiveCats.Add(cat);
        Activate();
    }
}
