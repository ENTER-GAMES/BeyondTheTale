using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Resetter : MonoBehaviour
{
    private static Resetter instance;

    [SerializeField]
    private float targetNobodyTime;     // 아무도 없는 시간의 목표값
    private float nobodyTimer;          // 아무도 없는(그림자가 없는) 시간을 세는 타이머

    private string prevSceneName;

    [SerializeField]
    private string targetSceneName;

    private void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
            {
                StopAllCoroutines();

                if (prevSceneName != scene.name)
                {
                    // 씬 이름 저장
                    prevSceneName = scene.name;
                    // 새로운 씬이면 타이머 초기화
                    nobodyTimer = 0;
                }

                // 타겟 씬이 아닐 때, 타이머 시작
                Scene currentScene = SceneManager.GetActiveScene();
                if (currentScene.name != targetSceneName)
                    StartCoroutine(CheckShadowRoutine());
            };

        Scene currentScene = SceneManager.GetActiveScene();
        prevSceneName = currentScene.name;

        SceneManager.LoadScene(currentScene.name);
    }

    private IEnumerator CheckShadowRoutine()
    {
        // 0.5초 정도는 검은 화면으로 인해 그림자로 인식될 수 있기 때문에 시간에 포함하지 않는다.
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            nobodyTimer += Time.deltaTime;

            if (MeshDrawer.ShadowCount > 0)
            {
                nobodyTimer = 0;
            }
            else if (nobodyTimer >= targetNobodyTime)
            {
                Reset();
                yield break;
            }

            yield return null;
        }
    }

    private void Reset()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
