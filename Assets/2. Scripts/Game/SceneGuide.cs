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

    private void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        textExplain = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
            {
                if (prevSceneName == scene.name) return;

                prevSceneName = scene.name;

                StopAllCoroutines();

                if (FindExplainBySceneName(scene.name, out SceneExplain explain))
                    StartCoroutine(ExplainRoutine(explain));
            };

        Scene currentScene = SceneManager.GetActiveScene();
        prevSceneName = currentScene.name;

        if (FindExplainBySceneName(currentScene.name, out SceneExplain explain))
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
        foreach (SceneExplainText explainText in explain.explainTexts)
        {
            textExplain.text = explainText.explainText;
            yield return new WaitForSeconds(explainText.displayTime);
        }

        textExplain.text = "";
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
    }
}
