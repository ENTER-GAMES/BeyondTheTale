using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneGuide : MonoBehaviour
{
    private static SceneGuide instance;

    [SerializeField]
    private SceneExplain[] explains;

    private string prevSceneName;

    [SerializeField]
    private float charDelayTime;

    private TextMeshProUGUI textExplain;

    private AudioSource audioSource;

    private void Awake()
    {
        textExplain = GetComponentInChildren<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();

        if (instance)
            return;

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (instance != this) return;

        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
            {
                if (prevSceneName == scene.name) return;

                prevSceneName = scene.name;

                textExplain.text = "";

                StopAllCoroutines();

                ExplainBySceneName(scene.name);
            };

        Scene currentScene = SceneManager.GetActiveScene();
        prevSceneName = currentScene.name;

        ExplainBySceneName(currentScene.name);
    }

    public void ExplainBySceneName(string sceneName)
    {
        if (FindExplainBySceneName(sceneName, out SceneExplain explain))
            StartCoroutine(ExplainRoutine(explain));
    }

    private bool FindExplainBySceneName(string sceneName, out SceneExplain explain)
    {
        foreach (SceneExplain _explain in explains)
        {
            if (_explain.targetSceneName == sceneName)
            {
                explain = _explain;
                return true;
            }
        }

        explain = new SceneExplain();
        return false;
    }

    private IEnumerator ExplainRoutine(SceneExplain explain)
    {
        // 처음에만 오디오 재생
        foreach (SceneExplainText explainText in explain.explainTexts)
        {
            // 나타남
            textExplain.text = explainText.explainText;
            yield return StartCoroutine(TwinkleTextRoutine(0, 1, 0.3f));

            // 오디오 재생, 기다림
            audioSource.PlayOneShot(explainText.explainAudio);
            yield return new WaitForSeconds(explainText.displayTime);

            // 사라짐
            yield return StartCoroutine(TwinkleTextRoutine(1, 0, 0.3f));
        }

        while (true)
        {
            // 처음에만 오디오 재생
            foreach (SceneExplainText explainText in explain.explainTexts)
            {
                // 나타남
                textExplain.text = explainText.explainText;
                yield return StartCoroutine(TwinkleTextRoutine(0, 0.4f, 1));

                // 기다림
                yield return new WaitForSeconds(1);

                // 사라짐
                yield return StartCoroutine(TwinkleTextRoutine(0.4f, 0, 1));
            }

            yield return null;
        }
    }

    private IEnumerator TwinkleTextRoutine(float fromA, float toA, float twinkleTime)
    {
        float timer = 0, percent = 0;

        while (percent < 1)
        {
            timer += Time.deltaTime;
            percent = timer / twinkleTime;

            Color newColor = textExplain.color;
            newColor.a = Mathf.Lerp(fromA, toA, percent);
            textExplain.color = newColor;

            yield return null;
        }
    }

    [Serializable]
    private struct SceneExplain
    {
        public string targetSceneName;              // 목표 씬 이름
        public SceneExplainText[] explainTexts;
    }

    [Serializable]
    private struct SceneExplainText
    {
        public float displayTime;       // 설명 텍스트가 보여질 시간
        public string explainText;      // 설명 텍스트
        public AudioClip explainAudio;  // 설명 오디오
    }
}
