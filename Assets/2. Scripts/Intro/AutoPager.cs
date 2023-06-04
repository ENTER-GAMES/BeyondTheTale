using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoPager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> shadowObjects;     // 자동으로 책장 열어줄 그림자 리스트
    [SerializeField]
    private GameObject shadowDetector;

    private void Awake()
    {
        int targetPage = PlayerPrefs.GetInt("targetPage", 0);

        // targetPage == -1이면, 그림자 인식기 활성화
        if (targetPage < 0)
        {
            Debug.Log("AutoPager: 수동 페이지 넘기기");
            shadowDetector?.SetActive(true);
            return;
        }
        else
        {
            Debug.Log("AutoPager: 자동 페이지 넘기기");
            OpenTargetPage(targetPage);
        }
    }

    private void OpenTargetPage(int targetPage)
    {
        DeactivateAllShadowObject();
        ActivateShadowObjectByTargetPage(targetPage);
    }

    private void DeactivateAllShadowObject()
    {
        foreach (var shadowObject in shadowObjects)
            shadowObject.SetActive(false);
    }

    private void ActivateShadowObjectByTargetPage(int targetPage)
    {
        shadowObjects[targetPage].SetActive(true);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Minus)) return;

        int currentTargetPage = PlayerPrefs.GetInt("targetPage", 0);

        int newTargetPage = currentTargetPage >= 0 ? -1 : 0;
        PlayerPrefs.SetInt("targetPage", newTargetPage);

        // 씬 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetTargetPage(int targetPage)
    {
        PlayerPrefs.SetInt("targetPage", targetPage);
    }
}
