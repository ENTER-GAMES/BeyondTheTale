using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private CameraBasedShadowDetector detector;
    private UIManager uiManager;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        detector = FindObjectOfType<CameraBasedShadowDetector>();
        uiManager = FindObjectOfType<UIManager>();
    }

    public void LoadIntroScene(float delay)
    {
        Invoke(nameof(PrivateLoadIntroScene), delay);
    }

    private void PrivateLoadIntroScene()
    {
        detector?.Dispose();
        SceneManager.LoadScene("Intro");
    }

    private void Update()
    {
        if (uiManager == null || !uiManager.IsUIOpen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                detector?.Dispose();
                SceneManager.LoadScene("Intro");
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                detector?.Dispose();
                SceneManager.LoadScene("Chapter 1");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                detector?.Dispose();
                SceneManager.LoadScene("Chapter 2");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                detector?.Dispose();
                SceneManager.LoadScene("Chapter 4");
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                detector?.Dispose();
                SceneManager.LoadScene("Chapter 5-1");
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                detector?.Dispose();
                SceneManager.LoadScene("Chapter 5-2");
            }
        }
    }
}
